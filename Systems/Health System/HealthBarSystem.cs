

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

    public sealed class HealthBarSystem : SystemBase
    {
        private const string HP_BAR_PREFAB_NAME  = "HealthBarPrefab";
        private const string HP_BAR_FILL_GO_NAME = "BarFill";

        private static HealthBarSystem _instance;
        private static GameObject      _hpBarPrefab;
        private static Transform       _mainCameraTransform;

        private static bool healthBarsEnabled = true;

        private static event OnObjectChange<bool> ToggleHealthBars;

        public static HealthBarSystem current { get => _instance; }

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

        internal static bool healthChangesUpdated { set => _instance.updateVisualHealthChanges = value; }

        public HealthBarSystem()
        {
            _hpBarPrefab = Resources.Load<GameObject>(HP_BAR_PREFAB_NAME);
            _mainCameraTransform = Camera.main.transform;

            activeHealthBars = new HashSet<HealthBar>(GameObject.FindObjectsOfType<HealthBar>(false));

            int hbLength = activeHealthBars.Count;

            HealthBar.OnComponentCreate  += OnHealthBarCreatedUpdateCache;
            HealthBar.OnComponentDestroy += OnHealthBarDestroyedUpdateCache;
            HealthBar.OnComponentEnable  += OnHealthBarEnable;
            HealthBar.OnComponentDisable += OnHealthBarDisable;

            _cacheHealthBars = new HealthBar[hbLength];
            _cacheHealthBarData = new HealthBarData[hbLength];

            healthBarAnchorList = new TransformAccessArray(hbLength);
            healthBarTransformList = new TransformAccessArray(hbLength);
            healthBarFillTransformList = new TransformAccessArray(hbLength);

            activeHealthBars.CopyTo(_cacheHealthBars);

            int i;
            int length = _cacheHealthBars.Length;
            for (i = 0; i < length; i++)
            {
                HealthBar healthBar = _cacheHealthBars[i];

                healthBar._id = i;

                _cacheHealthBars[i] = healthBar;
                _cacheHealthBarData[i] = new HealthBarData(in healthBar);

                healthBar.generatedHealthBar = GameObject.Instantiate(_hpBarPrefab, healthBar.barAnchor.position, Quaternion.identity);
#if UNDER_DEVELOPMENT
                healthBar.generatedHealthBar.name = $"[Auto-Generated] {healthBar.gameObject.name} Health Bar";
#endif
                healthBar.generatedHealthBar.transform.localScale = healthBar.barScale;
                healthBar.generatedHealthBar.transform.forward = -_mainCameraTransform.forward;

                Transform barFillTransform = healthBar.generatedHealthBar.transform.Find(HP_BAR_FILL_GO_NAME);

                healthBarAnchorList.Add(healthBar.barAnchor);
                healthBarTransformList.Add(healthBar.generatedHealthBar.transform);
                healthBarFillTransformList.Add(barFillTransform); 
            }             

            locked = length == 0;
        }
        ~HealthBarSystem()
        {
            _instance = null;
            Dispose(false);
        }


        HashSet<HealthBar> activeHealthBars;

        bool locked;
        bool transformChangesUpdated;
        bool updateVisualHealthChanges;

        // --- Cache data --- //

        HealthBar[]     _cacheHealthBars;
        HealthBarData[] _cacheHealthBarData;

        TransformAccessArray healthBarAnchorList;
        TransformAccessArray healthBarTransformList;
        TransformAccessArray healthBarFillTransformList;

        // --- Cache data --- //

        private void OnHealthBarCreatedUpdateCache(HealthBar healthBar)
        {
            locked = true;

            if (activeHealthBars.Add(healthBar))
            {
                int length = activeHealthBars.Count;

                Array.Resize(ref _cacheHealthBars, length);
                Array.Resize(ref _cacheHealthBarData, length);
                healthBarAnchorList.capacity = length;
                healthBarTransformList.capacity = length;
                healthBarFillTransformList.capacity = length;

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
                ToggleHealthBars -= healthBar.generatedHealthBar.SetActive;
                GameObject.Destroy(healthBar.generatedHealthBar);

                int length = activeHealthBars.Count;

                Array.Resize(ref _cacheHealthBars, length);
                Array.Resize(ref _cacheHealthBarData, length);
                healthBarAnchorList.capacity = length;
                healthBarTransformList.capacity = length;
                healthBarFillTransformList.capacity = length;

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
                if (healthBarAnchorList.isCreated)
                    healthBarAnchorList.Dispose();

                if (healthBarTransformList.isCreated)
                    healthBarTransformList.Dispose();

                if (healthBarFillTransformList.isCreated)
                    healthBarFillTransformList.Dispose();
            }

            Resources.UnloadUnusedAssets();

            _instance            = null;
            _hpBarPrefab         = null;
            _mainCameraTransform = null;

            activeHealthBars = null;

            _cacheHealthBars    = null;
            _cacheHealthBarData = null;

            HealthBar.OnComponentCreate  -= OnHealthBarCreatedUpdateCache;
            HealthBar.OnComponentDestroy -= OnHealthBarDestroyedUpdateCache;
            HealthBar.OnComponentEnable  -= OnHealthBarEnable;
            HealthBar.OnComponentDisable -= OnHealthBarDisable;

            base.Dispose(disposing);
        }

        public unsafe override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
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

            if (updateVisualHealthChanges)
            {
                NativeArray<HealthData> healthData = HealthSystem.current.GetHealthData();

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
