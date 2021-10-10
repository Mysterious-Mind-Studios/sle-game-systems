

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
        private const string HP_BAR_PREFAB_NAME  = "HealthBarPrefab";
        private const string HP_BAR_FILL_GO_NAME = "BarFill";

        private static HealthSystem _instance;
        private static Transform    _mainCameraTransform;
        private static GameObject   _hpBarPrefab;

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
                ToggleHealthBars(healthBarsEnabled);
            }
        }

        public HealthSystem()
        {
            if (_instance)
                _instance.Dispose();

            _instance = this;

            _hpBarPrefab         = Resources.Load<GameObject>(HP_BAR_PREFAB_NAME);
            _mainCameraTransform = Camera.main.transform;
            
            activeHealths    = new HashSet<Health>(GameObject.FindObjectsOfType<Health>());
            activeHealthBars = new HashSet<HealthBar>(GameObject.FindObjectsOfType<HealthBar>());
            
            int hLength  = activeHealths.Count;
            int hbLength = activeHealthBars.Count;
            
            _cacheHealths    = new Health[hLength];
            _cacheHealthData = new HealthData[hLength];

            _cacheHealthBars    = new HealthBar[hbLength];
            _cacheHealthBarData = new HealthBarData[hbLength];

            healthBarAnchorList    = new TransformAccessArray(hbLength);
            healthBarTransformList = new TransformAccessArray(hbLength);
            healthBarFillTransformList = new TransformAccessArray(hbLength);

            activeHealths.CopyTo(_cacheHealths);

            int i;
            int healthIndex = 0;
            int healthBarIndex = 0;
            for (i = 0; i < hLength; i++)
            {
                Health health = _cacheHealths[i];

                if (health.enabled)
                {
                    health._id = healthIndex;
                    _cacheHealthData[healthIndex] = new HealthData(health);

                    HealthBar healthBar = health.GetComponent<HealthBar>();
                    if (healthBar && healthBar.enabled)
                    {
                        healthBar._id = healthBarIndex;
                        healthBar.healthComponentID = healthIndex;
                        healthBar.generatedHealthBar = GameObject.Instantiate(_hpBarPrefab, healthBar.barAnchor.position, Quaternion.identity);

                        Transform barFillTransform = healthBar.generatedHealthBar.transform.Find(HP_BAR_FILL_GO_NAME);

#if UNITY_EDITOR
                        healthBar.generatedHealthBar.name = $"[Auto-Generated] {healthBar.gameObject.name} Health Bar";
#endif
                        healthBar.generatedHealthBar.transform.localScale = healthBar.barScale;

                        if (healthBar.billboard)
                            healthBar.generatedHealthBar.transform.forward = -_mainCameraTransform.forward;

                        _cacheHealthBars[healthBarIndex] = healthBar;
                        _cacheHealthBarData[healthBarIndex] = new HealthBarData(in healthBar);

                        healthBarAnchorList.Add(healthBar.barAnchor);
                        healthBarTransformList.Add(healthBar.generatedHealthBar.transform);
                        healthBarFillTransformList.Add(barFillTransform);

                        ToggleHealthBars += healthBar.generatedHealthBar.gameObject.SetActive;

                        healthBarIndex++;
                    }

                    healthIndex++;
                }
            }

            Health.OnHealthChange += OnHealthChangeSignalUpdate;

            Resources.UnloadUnusedAssets();
        }
        ~HealthSystem()
        {
            _instance = null;
            Dispose(false);
        }

        HashSet<Health>    activeHealths;
        HashSet<HealthBar> activeHealthBars;

        bool detectedChanges = false;

        // --- Cache data --- //

        Health[]    _cacheHealths;
        HealthBar[] _cacheHealthBars;
        Transform[] _cacheGeneratedBars; // Just to keep track of the generated bars and prevent them from being collected.

        HealthData[]    _cacheHealthData;
        HealthBarData[] _cacheHealthBarData;

        TransformAccessArray healthBarAnchorList;
        TransformAccessArray healthBarTransformList;
        TransformAccessArray healthBarFillTransformList;

        // --- Cache data --- //

        private void OnHealthChangeSignalUpdate(Health health)
        {
            ref HealthData healthData = ref _cacheHealthData[health._id];

            if(health._currentHealthPoints == 0)
            {
                switch(health.onZeroHealth)
                {

                }
            }

            healthData = new HealthData(health);
            detectedChanges = true;
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

            Health.OnHealthChange -= OnHealthChangeSignalUpdate;

            base.Dispose(disposing);
        }

        public override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        {
            if (!healthBarsEnabled) return handle;
            if (!detectedChanges)   return handle;
            
            int length = _cacheHealthBars.Length;
            
            fixed (HealthBarData* healthBarDataPtr = &_cacheHealthBarData[0])
            {
                UpdateHealthBarDataJob updateHealthBarDataJob = new UpdateHealthBarDataJob
                {
                    healthBarDataPtr = healthBarDataPtr
                };

                JobHandle updateHealthBarDataJobhandle = updateHealthBarDataJob.Schedule(healthBarAnchorList, handle);
            
                UpdateHealthBarTransformJob updateHealthBarsTransformJob = new UpdateHealthBarTransformJob
                {
                    healthBarDataPtr  = healthBarDataPtr,
                    mainCameraForward = _mainCameraTransform.forward
                };
            
                JobHandle updateHealthBarsTransformJobHandle = updateHealthBarsTransformJob.Schedule(healthBarTransformList, updateHealthBarDataJobhandle);

                fixed (HealthData* healthDataPtr = &_cacheHealthData[0])
                {
                    UpdateHealthBarFillTransformJob updateHealthBarFillTransformJob = new UpdateHealthBarFillTransformJob
                    {
                        healthDataPtr = healthDataPtr
                    };

                    handle = updateHealthBarFillTransformJob.Schedule(healthBarFillTransformList, updateHealthBarsTransformJobHandle);

                    detectedChanges = false;

                    return handle;
                }
            }
        }
    }
}
