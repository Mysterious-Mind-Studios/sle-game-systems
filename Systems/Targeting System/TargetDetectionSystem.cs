
using System;
using System.Collections.Generic;

using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

namespace SLE.Systems.Targeting
{
    using SLE.Systems.Targeting.Data;
    using SLE.Systems.Targeting.Jobs;

    public unsafe class TargetDetectionSystem : SystemBase
    {
        static TargetDetectionSystem _instance = null;
        public static TargetDetectionSystem current => _instance;

        public TargetDetectionSystem()
        {
            if (_instance)
                _instance.Dispose();

            _instance = this;

            activeDetectors   = new HashSet<TargetDetector>(GameObject.FindObjectsOfType<TargetDetector>());
            activeTargetables = new HashSet<Targetable>(GameObject.FindObjectsOfType<Targetable>());

            int i;
            int detLength  = activeDetectors.Count;
            int tgtbLength = activeTargetables.Count; 

            TargetDetector.OnComponentCreate  += OnDetectorCreatedUpdateCache;
            TargetDetector.OnComponentDestroy += OnDetectorDestroyedUpdateCache;
            TargetDetector.OnComponentEnable  += OnDetectorEnableUpdateState;
            TargetDetector.OnComponentDisable += OnDetectorDisableUpdateState;

            Targetable.OnComponentCreate  += OnTargetableCreatedUpdateCache;
            Targetable.OnComponentDestroy += OnTargetableDestroyedUpdateCache;
            Targetable.OnComponentEnable  += OnTargetableEnableUpdateState;
            Targetable.OnComponentDisable += OnTargetableDisableUpdateState;

            _cacheDetectors      = new TargetDetector[detLength];
            _cacheTargetables    = new Targetable[tgtbLength];
            _cacheDetectorData   = new DetectorData[detLength];
            _cacheTargetableData = new TargetData[tgtbLength];

            activeDetectors.CopyTo(_cacheDetectors);
            activeTargetables.CopyTo(_cacheTargetables);

            int n = detLength > tgtbLength ? detLength : tgtbLength;
            int d = 0;
            int t = 0;
            for (i = 0; i < n; i++)
            {
                if(d++ < detLength)
                {
                    _cacheDetectors[i]._id = i;
                    _cacheDetectorData[i] = new DetectorData(in _cacheDetectors[i]);
                }

                if (t++ < tgtbLength)
                {
                    _cacheTargetables[i]._id = i;
                    _cacheTargetableData[i] = new TargetData(in _cacheTargetables[i]);
                }
            }


            locked = detLength == 0;
            shouldRunUpdate = false;
        }
        ~TargetDetectionSystem()
        {
            _instance = null;
            Dispose(false);
        }

        HashSet<TargetDetector> activeDetectors;
        HashSet<Targetable>     activeTargetables;

        bool locked;
        bool shouldRunUpdate;

        // --- Cache data --- //

        TargetDetector[] _cacheDetectors;
        Targetable[]     _cacheTargetables;
        DetectorData[]   _cacheDetectorData;
        TargetData[]     _cacheTargetableData;

        // --- Cache data --- //


        private void OnDetectorCreatedUpdateCache(TargetDetector detector)
        {
            locked = true;

            if (activeDetectors.Add(detector))
            {
                int length = activeDetectors.Count;

                Array.Resize(ref _cacheDetectors, length);
                Array.Resize(ref _cacheDetectorData, length);

                activeDetectors.CopyTo(_cacheDetectors);

                int i;
                for (i = 0; i < length; i++)
                {
                    _cacheDetectors[i]._id = i;
                    _cacheDetectorData[i] = new DetectorData(_cacheDetectors[i]);
                }
            }

            locked = false;
        }
        private void OnDetectorDestroyedUpdateCache(TargetDetector detector)
        {
            locked = true;

            if (activeDetectors.Remove(detector))
            {
                int length = activeDetectors.Count;

                Array.Resize(ref _cacheDetectors, length);
                Array.Resize(ref _cacheDetectorData, length);

                activeDetectors.CopyTo(_cacheDetectors);

                int i;
                for (i = 0; i < length; i++)
                {
                    _cacheDetectors[i]._id = i;
                    _cacheDetectorData[i]  = new DetectorData(_cacheDetectors[i]);
                }
            }

            locked = false;
        }
        private void OnDetectorEnableUpdateState(TargetDetector detector)
        {
            locked = true;

            int index = detector._id;
            ref DetectorData data = ref _cacheDetectorData[index];

            data.state = DetectorState.Active;

            locked = false;
        }
        private void OnDetectorDisableUpdateState(TargetDetector detector)
        {
            locked = true;

            int index = detector._id;

            ref DetectorData data = ref _cacheDetectorData[index];

            data.state = DetectorState.Inactive;

            locked = false;
        }
        private void OnDetectorSetTargetUpdateState(TargetDetector detector)
        {
            locked = true;

            int index = detector._id;
            ref DetectorData data = ref _cacheDetectorData[index];

            data.state = DetectorState.Active;

            locked = false;
        }
        private void OnDetectorClearTargetUpdateState(TargetDetector detector)
        {
            locked = true;

            int index = detector._id;
            ref DetectorData data = ref _cacheDetectorData[index];

            data.state = detector.enabled ? DetectorState.Active : DetectorState.Inactive;

            locked = false;
        }
        private void OnTargetableCreatedUpdateCache(Targetable targetable)
        {
            locked = true;

            if (activeTargetables.Add(targetable))
            {
                int length = activeTargetables.Count;

                Array.Resize(ref _cacheTargetables, length);
                Array.Resize(ref _cacheTargetableData, length);

                activeTargetables.CopyTo(_cacheTargetables);

                int i;
                for (i = 0; i < length; i++)
                {
                    _cacheTargetables[i]._id = i;
                    _cacheTargetableData[i]  = new TargetData(_cacheTargetables[i]);
                }
            }

            locked = false;
        }
        private void OnTargetableDestroyedUpdateCache(Targetable targetable)
        {
            locked = true;

            if (activeTargetables.Remove(targetable))
            {
                int length = activeTargetables.Count;

                Array.Resize(ref _cacheTargetables, length);
                Array.Resize(ref _cacheTargetableData, length);

                activeTargetables.CopyTo(_cacheTargetables);

                int i;
                for (i = 0; i < length; i++)
                {
                    _cacheTargetables[i]._id = i;
                    _cacheTargetableData[i]  = new TargetData(_cacheTargetables[i]);
                }
            }

            locked = false;
        }
        private void OnTargetableEnableUpdateState(Targetable targetable)
        {
            locked = true;

            int index = targetable._id;
            ref TargetData data = ref _cacheTargetableData[index];

            data.state = TargetState.Valid;

            locked = false;
        }
        private void OnTargetableDisableUpdateState(Targetable targetable)
        {
            locked = true;

            int index = targetable._id;
            ref TargetData data = ref _cacheTargetableData[index];

            data.state = TargetState.Invalid;

            locked = false;
        }

        protected override void Dispose(bool disposing)
        {
            _instance = null;
            activeDetectors = null;
            activeTargetables = null;
            _cacheDetectors = null;
            _cacheTargetables = null;
            _cacheDetectorData = null;
            _cacheTargetableData = null;

            base.Dispose(disposing);
        }

        public override void OnStop()
        {
            TargetDetector.OnComponentCreate  -= OnDetectorCreatedUpdateCache;
            TargetDetector.OnComponentDestroy -= OnDetectorDestroyedUpdateCache;
            TargetDetector.OnComponentEnable  -= OnDetectorEnableUpdateState;
            TargetDetector.OnComponentDisable -= OnDetectorDisableUpdateState;
            Targetable.OnComponentCreate  -= OnTargetableCreatedUpdateCache;
            Targetable.OnComponentDestroy -= OnTargetableDestroyedUpdateCache;
            Targetable.OnComponentEnable  -= OnTargetableEnableUpdateState;
            Targetable.OnComponentDisable -= OnTargetableDisableUpdateState;
        }

        public override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        {
            if (locked) return handle;

            int i;
            int dLength = _cacheDetectors.Length;
            int tLength = _cacheTargetables.Length;

            if (dLength == 0 || 
                tLength == 0)
            {
                locked = true;
                return handle;
            }

            int batchCount = GetBatchCount(dLength);

            fixed (DetectorData* detectorDataPtr = &_cacheDetectorData[0])
            {
                fixed (TargetData* targetDataPtr = &_cacheTargetableData[0])
                {
                    int n = dLength > tLength ? dLength : tLength;
                    int d = 0;
                    int t = 0;
                    for (i = 0; i < n; i++)
                    {
                        if (d++ < dLength)
                        {
                            switch (detectorDataPtr[i].state)
                            {
                                case DetectorState.Active:
                                    detectorDataPtr[i].position = _cacheDetectors[i]._fovOrigin.position;
                                    break;

                                case DetectorState.HasFixedTarget:
                                    {
                                        if (detectorDataPtr[i].state == DetectorState.HasFixedTarget)
                                        {
                                            if (_cacheDetectors[i]._target)
                                                continue;

                                            detectorDataPtr[i].state = DetectorState.Active;
                                        }
                                    }
                                    break;
                            }
                        }

                        if (t++ < tLength)
                            targetDataPtr[i].position = _cacheTargetables[i].aimPoint.position;
                    }

                    FindTargetJob findTargetJob = new FindTargetJob
                    {
                        detectorData = detectorDataPtr,
                        targetData = targetDataPtr,
                        targetDataLength = tLength,
                        time = time
                    };

                    handle = findTargetJob.Schedule(dLength, batchCount, handle);

                }
            }

            return handle;
        }
        public override void OnUpdate(float time, float deltaTime)
        {
            if (locked) return;

            int i;
            int index;
            int dLength = _cacheDetectors.Length;
            int tLength = _cacheTargetables.Length;

            fixed (DetectorData* detectorDataPtr = &_cacheDetectorData[0])
            {
                for (i = 0; i < dLength; i++)
                {
                    index = detectorDataPtr[i].targetIndex;

                    if (index >= 0 && index < tLength)
                    {
                        _cacheDetectors[i]._target = _cacheTargetables[index];
                        continue;
                    }

                    _cacheDetectors[i]._target = null;
                }
            }
        }
    }
}
