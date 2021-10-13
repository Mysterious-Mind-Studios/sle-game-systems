

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Jobs;

using Unity.Jobs;
using Unity.Collections;


namespace SLE.Systems.Health
{
    using SLE.Events;
    using SLE.Systems.Health.Data;
    using SLE.Systems.Health.Jobs;

    public unsafe sealed class HealthSystem : SystemBase
    {
        private const string HP_BAR_PREFAB_NAME  = "HealthBarPrefab";
        private const string HP_BAR_FILL_GO_NAME = "BarFill";

        private static HealthSystem _instance;
        private static GameObject   _hpBarPrefab;
        private static Transform    _mainCameraTransform;

        private static bool healthBarsEnabled = true;
        private static event OnObjectChange<bool> ToggleHealthBars;

        public static HealthSystem current => _instance;

        /// <summary>
        /// This is a reference to the first found 'MainCamera' game object. This value can be changed.
        /// </summary>
        public static Transform mainCameraTransform { get => _mainCameraTransform; }
        /// <summary>
        /// This is not an active GameObject. This is just a reference to the Prefab asset.
        /// </summary>
        public static GameObject healthBarPrefab { get => _hpBarPrefab; }

        public static bool displayHealthBars
        {
            get => healthBarsEnabled;
            set
            {
                healthBarsEnabled = value;
                ToggleHealthBars?.Invoke(healthBarsEnabled);
            }
        }

        public HealthSystem()
        {
            if (_instance)
                _instance.Dispose();

            _instance = this;

            _hpBarPrefab = Resources.Load<GameObject>(HP_BAR_PREFAB_NAME);
            _mainCameraTransform = Camera.main.transform;

            activeHealths = new HashSet<Health>(GameObject.FindObjectsOfType<Health>());
            activeHealthBars = new HashSet<HealthBar>();

            Health.OnHealthChange        += OnHealthChangeUpdateState;
            Health.OnComponentCreate     += OnHealthCreatedUpdateCache;
            Health.OnComponentDestroy    += OnHealthDestroyedUpdateCache;
            HealthBar.OnComponentCreate  += OnHealthBarCreatedUpdateCache;
            HealthBar.OnComponentDestroy += OnHealthBarDestroyedUpdateCache;
            HealthBar.OnComponentEnable  += OnHealthBarEnable;
            HealthBar.OnComponentDisable += OnHealthBarDisable;

            int length = activeHealths.Count;

            _cacheHealths    = new Health[length];
            _cacheHealthData = new HealthData[length];

            _cacheHealthBars    = new HealthBar[length];
            _cacheHealthBarData = new HealthBarData[length];

            healthBarTransformList = new TransformAccessArray(length);
            healthBarFillTransformList = new TransformAccessArray(length);

            activeHealths.CopyTo(_cacheHealths);

            int i;
            int j = 0;
            for (i = 0; i < length; i++)
            {
                Health health = _cacheHealths[i];

                health._id = i;
                _cacheHealthData[i] = new HealthData(health);

                HealthBar healthBar = health.GetComponent<HealthBar>();

                if (healthBar)
                {
                    if (activeHealthBars.Add(healthBar))
                    {
                        healthBar._id = j;

                        _cacheHealthBars[j]    = healthBar;
                        _cacheHealthBarData[j] = new HealthBarData(in healthBar);

                        healthBarTransformList.Add(healthBar.attachedPrefab);
                        healthBarFillTransformList.Add(healthBar.attachedPrefab.Find(HP_BAR_FILL_GO_NAME));
                        healthBar.attachedPrefab.transform.forward = -_mainCameraTransform.forward;

                        j++;
                    }
                }
            }

            locked = false;
        }
        ~HealthSystem()
        {
            _instance = null;
            Dispose(true);
        }


        HashSet<Health>    activeHealths;
        HashSet<HealthBar> activeHealthBars;

        bool locked;
        bool healthChangesUpdated;
        bool transformChangesUpdated;
        bool updateVisualHealthChanges;

        // --- Cache data --- //

        Health[]     _cacheHealths;
        HealthBar[]  _cacheHealthBars;

        HealthData[]    _cacheHealthData;
        HealthBarData[] _cacheHealthBarData;

        TransformAccessArray healthBarTransformList;
        TransformAccessArray healthBarFillTransformList;

        // --- Cache data --- //

        private void OnHealthChangeUpdateState(Health health)
        {
            if (!health) return;

            locked = true;

            ref HealthData healthData = ref _cacheHealthData[health._id];

            if (health._currentHealthPoints == 0)
            {
                switch (health.onZeroHealth)
                {
                    case HealthBehaviour.Disable:
                        {
                            HealthBar healthBar = health.GetComponent<HealthBar>();

                            if (healthBar)
                            {
                                ToggleHealthBars -= healthBar.attachedPrefab.gameObject.SetActive;

                                healthBar.attachedPrefab.gameObject.SetActive(false);
                                healthBar.enabled = false;
                            }

                            health.enabled = false;
                        }
                        break;

                    case HealthBehaviour.DisableGameObject:
                        {
                            HealthBar healthBar = health.GetComponent<HealthBar>();

                            if (healthBar)
                                ToggleHealthBars -= healthBar.attachedPrefab.gameObject.SetActive;

                            health.gameObject.SetActive(false);
                        }
                        break;

                    case HealthBehaviour.Destroy:
                        {
                            HealthBar healthBar = health.GetComponent<HealthBar>();

                            if (healthBar)
                            {
                                ToggleHealthBars -= healthBar.attachedPrefab.gameObject.SetActive;
                                GameObject.Destroy(healthBar);
                            }
                            
                            GameObject.Destroy(health);
                        }
                        break;

                    case HealthBehaviour.DestroyGameObject:
                        {
                            HealthBar healthBar = health.GetComponent<HealthBar>();

                            if (healthBar)
                                ToggleHealthBars -= healthBar.attachedPrefab.gameObject.SetActive;

                            GameObject.Destroy(health.gameObject);
                        }
                        break;

                    default:
                        break;
                }
            }

            healthData = new HealthData(health);
            updateVisualHealthChanges = true;

            locked = false;
        }

        private void OnHealthCreatedUpdateCache(Health health)
        {
            locked = true;

            if (activeHealths.Add(health))
            {
                int length = activeHealths.Count;

                Array.Resize(ref _cacheHealths, length);
                Array.Resize(ref _cacheHealthData, length);

                activeHealths.CopyTo(_cacheHealths);

                int i;
                for (i = 0; i < length; i++)
                {
                    Health _health = _cacheHealths[i];

                    _health._id = i;

                    _cacheHealthData[i] = new HealthData(_health);

                    HealthBar healthBar = health.GetComponent<HealthBar>();

                    if (healthBar)
                        healthBar.healthComponentID = i;
                }
            }

            locked = false;
        }
        private void OnHealthDestroyedUpdateCache(Health health)
        {
            locked = true;

            
            if (activeHealths.Remove(health))
            {
                int length = activeHealths.Count;

                Array.Resize(ref _cacheHealths, length);
                Array.Resize(ref _cacheHealthData, length);

                activeHealths.CopyTo(_cacheHealths);

                int i;
                for (i = 0; i < length; i++)
                {
                    Health _health = _cacheHealths[i];

                    _health._id = i;

                    _cacheHealthData[i] = new HealthData(_health);

                    HealthBar healthBar = health.GetComponent<HealthBar>();

                    if (healthBar)
                        healthBar.healthComponentID = i;
                }
            }

            locked = false;
        }
        private void OnHealthBarCreatedUpdateCache(HealthBar healthBar)
        {
            locked = true;

            if (activeHealthBars.Add(healthBar))
            {
                int length = activeHealthBars.Count;

                Array.Resize(ref _cacheHealthBars, length);
                Array.Resize(ref _cacheHealthBarData, length);

                healthBarTransformList.capacity = length;
                healthBarFillTransformList.capacity = length;
                
                // Testing: Clear the internal buffer so the index corresponds correctly for managed and unmanaged array ?
                healthBarTransformList.SetTransforms(null);
                // Testing: Clear the internal buffer so the index corresponds correctly for managed and unmanaged array ?
                healthBarFillTransformList.SetTransforms(null);

                activeHealthBars.CopyTo(_cacheHealthBars);

                int i;
                for (i = 0; i < length; i++)
                {
                    HealthBar _healthBar = _cacheHealthBars[i];

                    _healthBar._id = i;
                    _healthBar.healthComponentID = _healthBar.GetComponent<Health>()._id;

                    _cacheHealthBarData[i] = new HealthBarData(_healthBar);

                    if (healthBar.attachedPrefab)
                    {
                        healthBarTransformList.Add(_healthBar.attachedPrefab);
                        healthBarFillTransformList.Add(_healthBar.attachedPrefab.Find(HP_BAR_FILL_GO_NAME));
                    }
                }

                locked = false;
            }
        }
        private void OnHealthBarDestroyedUpdateCache(HealthBar healthBar)
        {
            locked = true;

            if (activeHealthBars.Remove(healthBar))
            {
                int index = healthBar._id;

                healthBarTransformList.RemoveAtSwapBack(index);
                healthBarFillTransformList.RemoveAtSwapBack(index);
                ToggleHealthBars -= healthBar.attachedPrefab.gameObject.SetActive;
                GameObject.Destroy(healthBar.attachedPrefab.gameObject);

                int length = activeHealthBars.Count;

                Array.Resize(ref _cacheHealthBars, length);
                Array.Resize(ref _cacheHealthBarData, length);

                healthBarTransformList.capacity = length;
                healthBarFillTransformList.capacity = length;

                // Testing: Clear the internal buffer so the index corresponds correctly for managed and unmanaged array ?
                healthBarTransformList.SetTransforms(null);
                // Testing: Clear the internal buffer so the index corresponds correctly for managed and unmanaged array ?
                healthBarFillTransformList.SetTransforms(null);

                activeHealthBars.CopyTo(_cacheHealthBars);

                int i;
                for (i = 0; i < length; i++)
                {
                    HealthBar _healthBar = _cacheHealthBars[i];

                    _healthBar._id = i;
                    _healthBar.healthComponentID = _healthBar.GetComponent<Health>()._id;

                    _cacheHealthBarData[i] = new HealthBarData(_healthBar);

                    if (healthBar.attachedPrefab)
                    {
                        healthBarTransformList.Add(_healthBar.attachedPrefab);
                        healthBarFillTransformList.Add(_healthBar.attachedPrefab.Find(HP_BAR_FILL_GO_NAME));
                    }
                }

                locked = false;
            }
        }
        private void OnHealthBarEnable(HealthBar healthBar)
        {
            healthBar.attachedPrefab.gameObject.SetActive((healthBar.enabled && displayHealthBars));
            ToggleHealthBars += healthBar.attachedPrefab.gameObject.SetActive;
        }
        private void OnHealthBarDisable(HealthBar healthBar)
        {
            healthBar.attachedPrefab.gameObject.SetActive(false);
            ToggleHealthBars -= healthBar.attachedPrefab.gameObject.SetActive;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (healthBarTransformList.isCreated)
                    healthBarTransformList.Dispose();

                if (healthBarFillTransformList.isCreated)
                    healthBarFillTransformList.Dispose();
            }

            _instance            = null;
            _hpBarPrefab         = null;
            _mainCameraTransform = null;

            activeHealths    = null;
            activeHealthBars = null;

            _cacheHealths       = null;
            _cacheHealthData    = null;
            _cacheHealthBars    = null;
            _cacheHealthBarData = null;

            Resources.UnloadUnusedAssets();

            base.Dispose(disposing);
        }

        public override void OnStop()
        {
            Health.OnHealthChange        -= OnHealthChangeUpdateState;
            Health.OnComponentCreate     -= OnHealthCreatedUpdateCache;
            Health.OnComponentDestroy    -= OnHealthDestroyedUpdateCache;
            HealthBar.OnComponentCreate  -= OnHealthBarCreatedUpdateCache;
            HealthBar.OnComponentDestroy -= OnHealthBarDestroyedUpdateCache;
            HealthBar.OnComponentEnable  -= OnHealthBarEnable;
            HealthBar.OnComponentDisable -= OnHealthBarDisable;
        }

        public unsafe override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        {
            if (locked) return handle;
            if (!healthBarsEnabled) return handle;

            int length = _cacheHealthBars.Length;

            if (length == 0)
            {
                locked = true;
                return handle;
            }

            int batchCount = GetBatchCount(length);

            //if (!detectedChanges) return handle;    // TODO - Turn functional when becomes possible to detect changes on transforms.
            fixed (HealthBarData* healthBarDataPtr = &_cacheHealthBarData[0])
            {
                UpdateHealthBarTransformJob updateHealthBarsTransformJob = new UpdateHealthBarTransformJob
                {
                    healthBarDataPtr = healthBarDataPtr,
                    mainCameraForward = _mainCameraTransform.forward
                };

                handle = updateHealthBarsTransformJob.Schedule(healthBarTransformList, handle);
            }

            if (updateVisualHealthChanges)
            {
                NativeArray<HealthData> healthData = new NativeArray<HealthData>(_cacheHealthData, Allocator.TempJob);

                UpdateHealthBarFillTransformJob updateHealthBarFillTransformJob = new UpdateHealthBarFillTransformJob
                {
                    healthDataArray = healthData
                };

                handle = updateHealthBarFillTransformJob.Schedule(healthBarFillTransformList, handle);

                updateVisualHealthChanges = false;
            }

            return handle;
        }
    }
}
