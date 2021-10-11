

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Jobs;

using Unity.Jobs;


namespace SLE.Systems.Health
{
    using SLE.Events;
    using SLE.Systems.Health.Data;
    using SLE.Systems.Health.Jobs;

    public unsafe sealed class HealthSystem : SystemBase
    {
        private const string HP_BAR_PREFAB_NAME = "HealthBarPrefab";
        private const string HP_BAR_FILL_GO_NAME = "BarFill";

        private static HealthSystem _instance;
        private static Transform _mainCameraTransform;
        private static GameObject _hpBarPrefab;

        private static bool healthBarsEnabled = true;

        private static event OnObjectChange<bool> ToggleHealthBars;

        /// <summary>
        /// This is a reference to the first found 'MainCamera' game object. This value can be changed.
        /// </summary>
        public static Transform mainCameraTransform { get => _mainCameraTransform; }
        /// <summary>
        /// This is not an active GameObject. This is just a reference to the Prefab asset.
        /// </summary>
        public static GameObject healthBarPrefab { get => _hpBarPrefab; }

        public static HealthSystem current => _instance;

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

            activeHealths = new HashSet<Health>(GameObject.FindObjectsOfType<Health>(false));
            activeHealthBars = new HashSet<HealthBar>(GameObject.FindObjectsOfType<HealthBar>(false));

            int hLength = activeHealths.Count;
            int hbLength = activeHealthBars.Count;

            Health.OnHealthChange     += OnHealthChangeUpdateState;
            Health.OnComponentCreate  += OnHealthCreatedUpdateCache;
            Health.OnComponentDestroy += OnHealthDestroyedUpdateCache;

            HealthBar.OnComponentCreate  += OnHealthBarCreatedUpdateCache;
            HealthBar.OnComponentDestroy += OnHealthBarDestroyedUpdateCache;
            HealthBar.OnComponentEnable  += OnHealthBarEnable;
            HealthBar.OnComponentDisable += OnHealthBarDisable;

            _cacheHealths = new Health[hLength];
            _cacheHealthData = new HealthData[hLength];

            _cacheHealthBars = new HealthBar[hbLength];
            _cacheHealthBarData = new HealthBarData[hbLength];

            healthBarAnchorList = new TransformAccessArray(hbLength);
            healthBarTransformList = new TransformAccessArray(hbLength);
            healthBarFillTransformList = new TransformAccessArray(hbLength);

            activeHealths.CopyTo(_cacheHealths);

            int i;
            int healthIndex = 0;
            int healthBarIndex = 0;
            for (i = 0; i < hLength; i++)
            {
                Health health = _cacheHealths[i];

                health._id = healthIndex;
                _cacheHealthData[healthIndex] = new HealthData(health);

                HealthBar healthBar = health.GetComponent<HealthBar>();

                // Health bars MUST have a Health component to be used. 
                // But the opposite does not apply.
                if (healthBar)
                {
                    if (healthBar.enabled)
                    {
                        healthBar._id = healthBarIndex;
                        healthBar.healthComponentID = healthIndex;

                        _cacheHealthBars[healthBarIndex] = healthBar;
                        _cacheHealthBarData[healthBarIndex] = new HealthBarData(in healthBar);

                        healthBar.generatedHealthBar = GameObject.Instantiate(_hpBarPrefab, healthBar.barAnchor.position, Quaternion.identity);
#if UNITY_EDITOR
                        healthBar.generatedHealthBar.name = $"[Auto-Generated] {healthBar.gameObject.name} Health Bar";
#endif
                        healthBar.generatedHealthBar.transform.localScale = healthBar.barScale;
                        healthBar.generatedHealthBar.transform.forward = -_mainCameraTransform.forward;

                        Transform barFillTransform = healthBar.generatedHealthBar.transform.Find(HP_BAR_FILL_GO_NAME);

                        healthBarAnchorList.Add(healthBar.barAnchor);
                        healthBarTransformList.Add(healthBar.generatedHealthBar.transform);
                        healthBarFillTransformList.Add(barFillTransform);

                        healthBarIndex++;
                    }
                    else
                    {
                        activeHealthBars.Remove(healthBar);
                    }
                }

                healthIndex++;
            }

            locked = hLength == 0;

            Resources.UnloadUnusedAssets();
        }
        ~HealthSystem()
        {
            _instance = null;
            Dispose(false);
        }

        HashSet<Health>    activeHealths;
        HashSet<HealthBar> activeHealthBars;

        bool locked                  = false;
        bool transformChangesUpdated = false;
        bool healthChangesUpdated    = true;

        // --- Cache data --- //

        Health[] _cacheHealths;
        HealthBar[] _cacheHealthBars;

        HealthData[] _cacheHealthData;
        HealthBarData[] _cacheHealthBarData;

        TransformAccessArray healthBarAnchorList;
        TransformAccessArray healthBarTransformList;
        TransformAccessArray healthBarFillTransformList;

        // --- Cache data --- //

        private void OnHealthChangeUpdateState(Health health)
        {
            if (!health) return;

            ref HealthData healthData = ref _cacheHealthData[health._id];

            if (health._currentHealthPoints == 0)
            {
                int index = health._id;

                switch (health.onZeroHealth)
                {
                    case HealthBehaviour.Disable:
                        {
                            HealthBar healthBar = _cacheHealthBars[index];

                            healthBar.generatedHealthBar.SetActive(false);
                            healthBar.enabled = false;
                            health.enabled = false;
                        }
                        break;

                    case HealthBehaviour.DisableGameObject:
                        {
                            HealthBar healthBar = _cacheHealthBars[index];

                            healthBar.generatedHealthBar.SetActive(false);
                            health.gameObject.SetActive(false);
                        }
                        break;

                    case HealthBehaviour.Destroy:
                        {
                            HealthBar healthBar = _cacheHealthBars[index];
                            GameObject.Destroy(healthBar.generatedHealthBar);
                            GameObject.Destroy(healthBar);
                            GameObject.Destroy(health);
                        }
                        break;

                    case HealthBehaviour.DestroyGameObject:
                        {
                            HealthBar healthBar = _cacheHealthBars[index];
                            GameObject.Destroy(healthBar.generatedHealthBar);
                            GameObject.Destroy(health.gameObject);
                        }
                        break;

                    default:
                        break;
                }
            }

            healthData = new HealthData(health);
            healthChangesUpdated = false;
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
                }
            }

            locked = false;
        }
        private void OnHealthDestroyedUpdateCache(Health health)
        {
            locked = true;

            int length = 0;
            if (activeHealths.Remove(health))
            {
                length = activeHealths.Count;

                Array.Resize(ref _cacheHealths, length);
                Array.Resize(ref _cacheHealthData, length);

                activeHealths.CopyTo(_cacheHealths);

                int i;
                for (i = 0; i < length; i++)
                {
                    Health _health = _cacheHealths[i];

                    _health._id = i;

                    _cacheHealthData[i] = new HealthData(_health);
                }
            }

            locked = length == 0;
        }
        private void OnHealthBarCreatedUpdateCache(HealthBar healthBar)
        {
            locked = true;

            if (activeHealthBars.Add(healthBar))
            {
                int length = activeHealthBars.Count;

                Array.Resize(ref _cacheHealthBars, length);
                Array.Resize(ref _cacheHealthBarData, length);
                healthBarAnchorList.capacity        = length; // Hope it resizes the internal buffer??
                healthBarTransformList.capacity     = length; // Hope it resizes the internal buffer??
                healthBarFillTransformList.capacity = length; // Hope it resizes the internal buffer??

                activeHealthBars.CopyTo(_cacheHealthBars);

                int i;
                for (i = 0; i < length; i++)
                {
                    HealthBar _healthBar = _cacheHealthBars[i];

                    _healthBar._id = i;
                    _healthBar.healthComponentID = _healthBar.GetComponent<Health>()._id;

                    _cacheHealthBarData[i] = new HealthBarData(_healthBar);

                    if (_healthBar.generatedHealthBar)
                        continue;

                    _healthBar.generatedHealthBar = GameObject.Instantiate(_hpBarPrefab, _healthBar.barAnchor.position, Quaternion.identity);
#if UNITY_EDITOR
                    _healthBar.generatedHealthBar.name = $"[Auto-Generated] {_healthBar.gameObject.name} Health Bar";
#endif
                    _healthBar.generatedHealthBar.transform.localScale = _healthBar.barScale;
                    _healthBar.generatedHealthBar.transform.forward = -_mainCameraTransform.forward;
                    _healthBar.generatedHealthBar.SetActive(false);

                    Transform barFillTransform = _healthBar.generatedHealthBar.transform.Find(HP_BAR_FILL_GO_NAME);

                    healthBarAnchorList.Add(_healthBar.barAnchor);
                    healthBarTransformList.Add(_healthBar.generatedHealthBar.transform);
                    healthBarFillTransformList.Add(barFillTransform);
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

                healthBarAnchorList.RemoveAtSwapBack(index);
                healthBarTransformList.RemoveAtSwapBack(index);
                healthBarFillTransformList.RemoveAtSwapBack(index);
                GameObject.Destroy(healthBar.generatedHealthBar);

                int length = activeHealthBars.Count;

                Array.Resize(ref _cacheHealthBars, length);
                Array.Resize(ref _cacheHealthBarData, length);
                healthBarAnchorList.capacity        = length; // Hope it resizes the internal buffer??
                healthBarTransformList.capacity     = length; // Hope it resizes the internal buffer??
                healthBarFillTransformList.capacity = length; // Hope it resizes the internal buffer??

                activeHealthBars.CopyTo(_cacheHealthBars);

                int i;
                for (i = 0; i < length; i++)
                {
                    HealthBar _healthBar = _cacheHealthBars[i];

                    _healthBar._id = i;
                    _healthBar.healthComponentID = _healthBar.GetComponent<Health>()._id;

                    _cacheHealthBarData[i] = new HealthBarData(_healthBar);

                    if (_healthBar.generatedHealthBar)
                        continue;

                    _healthBar.generatedHealthBar = GameObject.Instantiate(_hpBarPrefab, _healthBar.barAnchor.position, Quaternion.identity);
#if UNITY_EDITOR
                    _healthBar.generatedHealthBar.name = $"[Auto-Generated] {_healthBar.gameObject.name} Health Bar";
#endif
                    _healthBar.generatedHealthBar.transform.localScale = _healthBar.barScale;
                    _healthBar.generatedHealthBar.transform.forward = -_mainCameraTransform.forward;
                    _healthBar.generatedHealthBar.SetActive(false);

                    Transform barFillTransform = _healthBar.generatedHealthBar.transform.Find(HP_BAR_FILL_GO_NAME);

                    healthBarAnchorList.Add(_healthBar.barAnchor);
                    healthBarTransformList.Add(_healthBar.generatedHealthBar.transform);
                    healthBarFillTransformList.Add(barFillTransform);
                }

                locked = false;
            }
        }
        private void OnHealthBarEnable(HealthBar healthBar)
        {
            healthBar.generatedHealthBar.SetActive((healthBar.enabled && displayHealthBars));
            ToggleHealthBars += healthBar.generatedHealthBar.SetActive;
        }
        private void OnHealthBarDisable(HealthBar healthBar)
        {
            healthBar.generatedHealthBar.SetActive(false);
            ToggleHealthBars -= healthBar.generatedHealthBar.SetActive;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(healthBarAnchorList.isCreated)
                    healthBarAnchorList.Dispose();

                if (healthBarTransformList.isCreated)
                    healthBarTransformList.Dispose();

                if (healthBarFillTransformList.isCreated)
                    healthBarFillTransformList.Dispose();
            }

            _instance = null;
            _hpBarPrefab = null;
            _mainCameraTransform = null;

            activeHealths = null;
            activeHealthBars = null;

            _cacheHealths = null;
            _cacheHealthData = null;

            _cacheHealthBars = null;
            _cacheHealthBarData = null;

            Health.OnHealthChange     -= OnHealthChangeUpdateState;
            Health.OnComponentCreate  -= OnHealthCreatedUpdateCache;
            Health.OnComponentDestroy -= OnHealthDestroyedUpdateCache;

            HealthBar.OnComponentCreate  -= OnHealthBarCreatedUpdateCache;
            HealthBar.OnComponentDestroy -= OnHealthBarDestroyedUpdateCache;
            HealthBar.OnComponentEnable  -= OnHealthBarEnable;
            HealthBar.OnComponentDisable -= OnHealthBarDisable;

            base.Dispose(disposing);
        }

        public override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        {
            if (!healthBarsEnabled) return handle;
            if (locked) return handle;
            
            int length = _cacheHealthBars.Length;
            int batchCount = GetBatchCount(length);
            
            //if (!detectedChanges) return handle;    // TODO - Turn functional when becomes possible to detect changes on transforms.
            fixed (HealthBarData* healthBarDataPtr = &_cacheHealthBarData[0])
            {
                UpdateHealthBarDataJob updateHealthBarDataJob = new UpdateHealthBarDataJob
                {
                    healthBarDataPtr = healthBarDataPtr
                };

                handle = updateHealthBarDataJob.ScheduleReadOnly(healthBarAnchorList, batchCount, handle);
            
                UpdateHealthBarTransformJob updateHealthBarsTransformJob = new UpdateHealthBarTransformJob
                {
                    healthBarDataPtr  = healthBarDataPtr,
                    mainCameraForward = _mainCameraTransform.forward
                };

                handle = updateHealthBarsTransformJob.Schedule(healthBarTransformList, handle);
            }

            if (!healthChangesUpdated)
            {
                fixed (HealthData* healthDataPtr = &_cacheHealthData[0])
                {
                    UpdateHealthBarFillTransformJob updateHealthBarFillTransformJob = new UpdateHealthBarFillTransformJob
                    {
                        healthDataPtr = healthDataPtr
                    };

                    handle = updateHealthBarFillTransformJob.Schedule(healthBarFillTransformList, handle);
                }

                healthChangesUpdated = true;
            }

            return handle;
        }
    }
}
