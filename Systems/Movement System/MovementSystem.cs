
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
            activeMovers = new HashSet<Mover>(GameObject.FindObjectsOfType<Mover>());

            int length = activeMovers.Count;

            Mover.OnComponentCreate  += OnMoverCreateUpdateCache;
            Mover.OnComponentDestroy += OnMoverDestroyUpdateCache;
            Mover.OnComponentEnable  += OnMoverEnableUpdateData;
            Mover.OnComponentDisable += OnMoverDisableUpdateData;
            
            _cacheMovers        = new Mover[length];
            moversTransformList = new TransformAccessArray(length);

            activeMovers.CopyTo(_cacheMovers);

            int i;
            for (i = 0; i < length; i++)
            {
                Mover mover = _cacheMovers[i];

                mover._id = i;
                _cacheMoverData[i] = new MoverData(mover);

                moversTransformList.Add(mover.transform);
            }

            locked = length == 0;
        }
        ~MovementSystem()
        {
            _instance = null;
            Dispose(true);
        }


        HashSet<Mover> activeMovers;

        bool locked;

        // --- Cache Data --- //

        Mover[]     _cacheMovers;
        MoverData[] _cacheMoverData;

        TransformAccessArray moversTransformList;

        // --- Cache Data --- //

        private void OnMoverCreateUpdateCache(Mover mover)
        {
            locked = true;

            if (activeMovers.Add(mover))
            {
                int length = activeMovers.Count;

                Array.Resize(ref _cacheMovers, length);
                Array.Resize(ref _cacheMoverData, length);

                moversTransformList.capacity = length;
                moversTransformList.SetTransforms(null);

                activeMovers.CopyTo(_cacheMovers);

                int i;
                for (i = 0; i < length; i++)
                {
                    Mover _mover = _cacheMovers[i];

                    _mover._id = i;
                    _cacheMoverData[i] = new MoverData(_mover);

                    moversTransformList.Add(_mover.transform);
                }
            }

            locked = false;
        }
        private void OnMoverDestroyUpdateCache(Mover mover)
        {
            locked = true;

            if (activeMovers.Remove(mover))
            {
                int length = activeMovers.Count;

                Array.Resize(ref _cacheMovers, length);
                Array.Resize(ref _cacheMoverData, length);

                moversTransformList.capacity = length;
                moversTransformList.SetTransforms(null);

                activeMovers.CopyTo(_cacheMovers);

                int i;
                for (i = 0; i < length; i++)
                {
                    Mover _mover = _cacheMovers[i];

                    _mover._id = i;
                    _cacheMoverData[i] = new MoverData(_mover);

                    moversTransformList.Add(_mover.transform);
                }
            }

            locked = activeMovers.Count == 0;
        }
        private void OnMoverEnableUpdateData(Mover mover)
        {
            int index = mover._id;
            ref MoverData moverData = ref _cacheMoverData[index];

            moverData.direction = mover.direction;
        }
        private void OnMoverDisableUpdateData(Mover mover)
        {
            int index = mover._id;
            ref MoverData moverData = ref _cacheMoverData[index];

            moverData.direction = INVALID_DIRECTION;
        }

        public override void OnStop()
        {
            Mover.OnComponentCreate  -= OnMoverCreateUpdateCache;
            Mover.OnComponentDestroy -= OnMoverDestroyUpdateCache;
            Mover.OnComponentEnable  -= OnMoverEnableUpdateData;
            Mover.OnComponentDisable -= OnMoverDisableUpdateData;
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

            _cacheMovers = null;
            _cacheMoverData = null;

            base.Dispose(disposing);
        }

        public unsafe override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        {
            if (locked) return handle;

            fixed(MoverData* moverDataPtr = &_cacheMoverData[0])
            {
                return new ProcessMoverJob
                {
                    moverData = moverDataPtr,
                    deltaTime = deltaTime
                }.Schedule(moversTransformList);
            }
        }
    }
}
