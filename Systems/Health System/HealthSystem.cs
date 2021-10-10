
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Jobs;

using Unity.Jobs;


namespace SLE.Systems.Health
{
    using SLE.Systems.Health.Utils;
    using SLE.Systems.Health.Data;

    public unsafe sealed class HealthSystem : SystemBase
    {
        internal const string HP_BAR_PREFAB_NAME  = "HealthBar v2";
        internal const string HP_BAR_FILL_GO_NAME = "BarFill";

        private static HealthSystem _instance;
        private static Transform    _mainCameraTransform;
        private static GameObject   _hpBarPrefab;

        /// <summary>
        /// This is a reference to the first found 'MainCamera' game object. This value can be changed.
        /// </summary>
        internal static Transform mainCameraTransform { get => _mainCameraTransform; }
        /// <summary>
        /// This is not an active GameObject. This is just a reference to the Prefab asset.
        /// </summary>
        internal static GameObject healthBarPrefab { get => _hpBarPrefab; }

        public static bool displayHealthBars = true;

        public HealthSystem()
        {
            _instance = this;

            _hpBarPrefab         = Resources.Load<GameObject>(HP_BAR_PREFAB_NAME);
            _mainCameraTransform = Camera.main.transform;
            
            activeHealths    = new HashSet<Health>(GameObject.FindObjectsOfType<Health>());
            activeHealthBars = new HashSet<HealthBar>(GameObject.FindObjectsOfType<HealthBar>());
            
            int hLength  = activeHealths.Count;
            int hbLength = activeHealthBars.Count;
            
            _cacheHealths       = new Health[hLength];
            _cacheHealthBars    = new HealthBar[hbLength];
            _cacheHealthBarData = new HealthBarData[hbLength];

            healthBarAnchorList    = new TransformAccessArray(hbLength);
            healthBarTransformList = new TransformAccessArray(hbLength);

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

                    HealthBar healthBar = health.GetComponent<HealthBar>();
                    if (healthBar && healthBar.enabled)
                    {
                        Transform genBarObjectTransform = GameObject.Instantiate(_hpBarPrefab, healthBar.barAnchor.position, Quaternion.identity).transform;

                        healthBar._id = healthBarIndex;
                        healthBar.healthComponentID = healthIndex;

#if UNITY_EDITOR
                        genBarObjectTransform.name = $"[Auto-Generated] {healthBar.gameObject.name} Health Bar";
#endif
                        genBarObjectTransform.transform.localScale = healthBar.barScale;

                        if (healthBar.billboard)
                            genBarObjectTransform.transform.forward = -_mainCameraTransform.forward;

                        _cacheHealthBars[healthBarIndex] = healthBar;
                        _cacheHealthBarData[healthBarIndex] = new HealthBarData(in healthBar);

                        healthBarAnchorList.Add(healthBar.barAnchor);
                        healthBarTransformList.Add(genBarObjectTransform);

                        healthBarIndex++;
                    }

                    healthIndex++;
                }
            }

            Resources.UnloadUnusedAssets();
        }
        ~HealthSystem()
        {
            _instance = null;
            Dispose(false);
        }

        HashSet<Health>    activeHealths;
        HashSet<HealthBar> activeHealthBars;

        bool locked;
        bool shouldRunUpdate;

        // --- Cache data --- //

        Health[]    _cacheHealths;
        HealthBar[] _cacheHealthBars;

        HealthData[]    _cacheHealthData;
        HealthBarData[] _cacheHealthBarData;

        TransformAccessArray healthBarAnchorList;
        TransformAccessArray healthBarTransformList;

        // --- Cache data --- //

        private void OnHealthChangeSignalUpdate(Health health)
        {
            ref HealthData healthData = ref _cacheHealthData[health._id];

            if(health.maxHealth != healthData.max ||
               health.currentHealth != healthData.current)
            {
                healthData      = new HealthData(health);
                shouldRunUpdate = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(healthBarAnchorList.isCreated)
                    healthBarAnchorList.Dispose();

                if (healthBarTransformList.isCreated)
                    healthBarTransformList.Dispose();
            }

            _instance = null;
            _hpBarPrefab = null;
            _mainCameraTransform = null;
            activeHealths = null;
            activeHealthBars = null;
            _cacheHealths = null;
            _cacheHealthBars = null;
            _cacheHealthBarData = null; 

            base.Dispose(disposing);
        }

        public override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        {
            if (locked) return handle;
            
            int length = _cacheHealthBars.Length;
            
            fixed (HealthBarData* healthBarDataPtr = &_cacheHealthBarData[0])
            {
                UpdateHealthBarDataJob updateHealthBarDataJob = new UpdateHealthBarDataJob
                {
                    healthBarDataPtr = healthBarDataPtr
                };

                JobHandle updateHealthBarJobhandle = updateHealthBarDataJob.Schedule(healthBarAnchorList, handle);
            
                UpdateHealthBarTransformJob updateHealthBarsTransformJob = new UpdateHealthBarTransformJob
                {
                    healthBarDataPtr  = healthBarDataPtr,
                    mainCameraForward = _mainCameraTransform.forward
                };
            
                handle = updateHealthBarsTransformJob.Schedule(healthBarTransformList, updateHealthBarJobhandle);

                UpdateHealthBarFillTransformJob updateHealthBarFillTransformJob = new UpdateHealthBarFillTransformJob
                {

                };
            
                return handle;
            }
        }
    }
}
