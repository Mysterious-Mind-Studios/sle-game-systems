

using System;
using System.Collections.Generic;

using UnityEngine;

using Unity.Collections;


namespace SLE.Systems.Health
{
    using SLE.Systems.Health.Data;

    public unsafe sealed class HealthSystem : SystemBase
    {
        private static HealthSystem _instance;
        public static HealthSystem current => _instance;

        public HealthSystem()
        {
            if (_instance)
                _instance.Dispose();

            _instance = this;

            activeHealths = new HashSet<Health>(GameObject.FindObjectsOfType<Health>(false));

            int i;
            int length = activeHealths.Count;

            Health.OnHealthChange     += OnHealthChangeUpdateState;
            Health.OnComponentCreate  += OnHealthCreatedUpdateCache;
            Health.OnComponentDestroy += OnHealthDestroyedUpdateCache;

            _cacheHealths    = new Health[length];
            _cacheHealthData = new HealthData[length];

            activeHealths.CopyTo(_cacheHealths);

            for (i = 0; i < length; i++)
            {
                Health health = _cacheHealths[i];

                health._id = i;
                _cacheHealthData[i] = new HealthData(health);

                HealthBar healthBar = health.GetComponent<HealthBar>();

                if (healthBar)
                    healthBar.healthComponentID = i;
            }

            locked = length == 0;
        }
        ~HealthSystem()
        {
            _instance = null;
            Dispose(false);
        }


        HashSet<Health> activeHealths;

        bool locked               = false;
        bool healthChangesUpdated = true;


        // --- Cache data --- //

        Health[]     _cacheHealths;
        HealthData[] _cacheHealthData;

        // --- Cache data --- //

        internal NativeArray<HealthData> GetHealthData()
        {
            return new NativeArray<HealthData>(_cacheHealthData, Allocator.TempJob);
        }

        private void OnHealthChangeUpdateState(Health health)
        {
            if (!health) return;

            ref HealthData healthData = ref _cacheHealthData[health._id];

            if (health._currentHealthPoints == 0)
            {
                switch (health.onZeroHealth)
                {
                    case HealthBehaviour.Disable:
                        {
                            HealthBar healthBar = health.GetComponent<HealthBar>();

                            healthBar.generatedHealthBar.SetActive(false);
                            healthBar.enabled = false;
                            health.enabled = false;
                        }
                        break;

                    case HealthBehaviour.DisableGameObject:
                        {
                            HealthBar healthBar = health.GetComponent<HealthBar>();

                            healthBar.generatedHealthBar.SetActive(false);
                            health.gameObject.SetActive(false);
                        }
                        break;

                    case HealthBehaviour.Destroy:
                        {
                            HealthBar healthBar = health.GetComponent<HealthBar>();
                            GameObject.Destroy(healthBar.generatedHealthBar);
                            GameObject.Destroy(healthBar);
                            GameObject.Destroy(health);
                        }
                        break;

                    case HealthBehaviour.DestroyGameObject:
                        {
                            HealthBar healthBar = health.GetComponent<HealthBar>();
                            GameObject.Destroy(healthBar.generatedHealthBar);
                            GameObject.Destroy(health.gameObject);
                        }
                        break;

                    default:
                        break;
                }
            }

            healthData = new HealthData(health);
            HealthBarSystem.healthChangesUpdated = true;
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

                    HealthBar healthBar = health.GetComponent<HealthBar>();

                    if (healthBar)
                        healthBar.healthComponentID = i;
                }
            }

            locked = length == 0;
        }

        protected override void Dispose(bool disposing)
        {
            _instance = null;

            activeHealths = null;

            _cacheHealths = null;
            _cacheHealthData = null;

            Health.OnHealthChange     -= OnHealthChangeUpdateState;
            Health.OnComponentCreate  -= OnHealthCreatedUpdateCache;
            Health.OnComponentDestroy -= OnHealthDestroyedUpdateCache;

            base.Dispose(disposing);
        }
    }
}
