
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Jobs;

using Unity.Jobs;


namespace SLE.Systems.Movement
{
    using SLE.Systems.Movement.Data;
    using SLE.Systems.Movement.Jobs;

    public class MovementSystem : SystemBase
    {
        private static readonly Vector3 INVALID_DIRECTION = Vector3.positiveInfinity;
        private static MovementSystem _instance;

        public static MovementSystem current => _instance;

        public MovementSystem()
        {
            activeMovers = new HashSet<Movement>(GameObject.FindObjectsOfType<Movement>());

            int length = activeMovers.Count;

            Movement.OnComponentCreate  += OnMovementCreateUpdateCache;
            Movement.OnComponentDestroy += OnMovementDestroyUpdateCache;
            Movement.OnComponentEnable  += OnMovementEnableUpdateData;
            Movement.OnComponentDisable += OnMovementDisableUpdateData;
            
            _cacheMovements     = new Movement[length];
            _cacheMovementData  = new MovementData[length];

            moversTransformList = new TransformAccessArray(length);

            activeMovers.CopyTo(_cacheMovements);

            int i;
            for (i = 0; i < length; i++)
            {
                Movement mover = _cacheMovements[i];

                mover._id = i;
                _cacheMovementData[i] = new MovementData(mover);

                moversTransformList.Add(mover.transform);
            }

            locked = length == 0;
        }
        ~MovementSystem()
        {
            _instance = null;
            Dispose(true);
        }


        HashSet<Movement> activeMovers;

        bool locked;

        // --- Cache Data --- //

        Movement[]     _cacheMovements;
        MovementData[] _cacheMovementData;

        TransformAccessArray moversTransformList;

        // --- Cache Data --- //

        private void OnMovementCreateUpdateCache(Movement mover)
        {
            locked = true;

            if (activeMovers.Add(mover))
            {
                int length = activeMovers.Count;

                Array.Resize(ref _cacheMovements, length);
                Array.Resize(ref _cacheMovementData, length);

                moversTransformList.capacity = length;
                moversTransformList.SetTransforms(null);

                activeMovers.CopyTo(_cacheMovements);

                int i;
                for (i = 0; i < length; i++)
                {
                    Movement _mover = _cacheMovements[i];

                    _mover._id = i;
                    _cacheMovementData[i] = new MovementData(_mover);

                    moversTransformList.Add(_mover.transform);
                }
            }

            locked = false;
        }
        private void OnMovementDestroyUpdateCache(Movement mover)
        {
            locked = true;

            if (activeMovers.Remove(mover))
            {
                int length = activeMovers.Count;

                Array.Resize(ref _cacheMovements, length);
                Array.Resize(ref _cacheMovementData, length);

                moversTransformList.capacity = length;
                moversTransformList.SetTransforms(null);

                activeMovers.CopyTo(_cacheMovements);

                int i;
                for (i = 0; i < length; i++)
                {
                    Movement _mover = _cacheMovements[i];

                    _mover._id = i;
                    _cacheMovementData[i] = new MovementData(_mover);

                    moversTransformList.Add(_mover.transform);
                }
            }

            locked = activeMovers.Count == 0;
        }
        private void OnMovementEnableUpdateData(Movement mover)
        {
            int index = mover._id;
            ref MovementData moverData = ref _cacheMovementData[index];

            moverData = new MovementData(mover);
        }
        private void OnMovementDisableUpdateData(Movement mover)
        {
            int index = mover._id;
            ref MovementData moverData = ref _cacheMovementData[index];

            moverData.direction = INVALID_DIRECTION;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (moversTransformList.isCreated)
                    moversTransformList.Dispose();
            }

            _instance = null;

            activeMovers = null;

            _cacheMovements = null;
            _cacheMovementData = null;

            base.Dispose(disposing);
        }

        public override void OnStop()
        {
            Movement.OnComponentCreate  -= OnMovementCreateUpdateCache;
            Movement.OnComponentDestroy -= OnMovementDestroyUpdateCache;
            Movement.OnComponentEnable  -= OnMovementEnableUpdateData;
            Movement.OnComponentDisable -= OnMovementDisableUpdateData;
        }
        public unsafe override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        {
            if (locked) return handle;

            int length = _cacheMovements.Length;
            fixed(MovementData* moverDataPtr = &_cacheMovementData[0])
            {
                for (int i = 0; i < length; i++)
                {
                    moverDataPtr[i] = new MovementData(_cacheMovements[i]);
                }

                return new ProcessMoverJob
                {
                    moverData = moverDataPtr,
                    deltaTime = deltaTime
                }.Schedule(moversTransformList);
            }
        }
    }
}
