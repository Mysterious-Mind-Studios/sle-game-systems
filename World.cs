

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

using Unity.Jobs;
using Unity.Collections;

namespace SLE
{
	using SLE.Systems;

    [DisallowMultipleComponent]
    public sealed class World : MonoBehaviour
	{
		private static World _instance;
		public static World GetSceneWorld() => _instance;

        private static event Action<float, float> UpdateSystems;
        private static event Action<float, float> LateUpdateSystems;
        private static event Action StopAllSystems;
        private static event Action DisposeAllSystems;

		private SystemBase[] _systems = null;
        private NativeArray<JobHandle> jobHandleList;

#if UNDER_DEVELOPMENT
        [SerializeField]
        private bool allowPhysicsSimulation;
#else
        private bool allowPhysicsSimulation;
#endif

        private SystemBase[] InitSystems()
        {
            List<SystemBase> systemList = new List<SystemBase>();
            Assembly runtimeAssembly = Assembly.GetExecutingAssembly();

            var types = runtimeAssembly.GetTypes().Where(t =>
            {
                return t.BaseType == typeof(SystemBase);
            });

            foreach (var type in types)
            {
                SystemBase system = (SystemBase)Activator.CreateInstance(type);
                systemList.Add(system);

                UpdateSystems     += system.OnUpdate;
                LateUpdateSystems += system.OnLateUpdate;
                StopAllSystems    += system.OnStop;
                DisposeAllSystems += system.Dispose;
            }

            return systemList.ToArray();
        }

        private void Awake()
		{
			if (_instance && _instance != this)
            {
				Destroy(gameObject);
                return;
            }
	
			_instance = this;

			_systems = InitSystems();

            jobHandleList = new NativeArray<JobHandle>(_systems.Length, Allocator.Persistent);

            Physics.autoSimulation = allowPhysicsSimulation;
        }
        private void Update()
        {
            int length = _systems.Length;

            float time      = Time.time;
            float deltaTime = Time.deltaTime;

            JobHandle job = default;
            for (int i = 0; i < length; i++)
            {
                job = _systems[i].OnJobUpdate(time, deltaTime, ref job);
                jobHandleList[i] = job;
            }
            job.Complete();
            //JobHandle.CompleteAll(jobHandleList);

            time      = Time.time;
            deltaTime = Time.deltaTime;
            UpdateSystems(time, deltaTime);

            time      = Time.time;
            deltaTime = Time.deltaTime;
            LateUpdateSystems(time, deltaTime);
        }
        private void OnDestroy()
        {
            if (_instance != this)
                return;

            if(jobHandleList.IsCreated)
                jobHandleList.Dispose();

            StopAllSystems();

            int length = _systems.Length;

            for (int i = 0; i < length; i++)
            {
                UpdateSystems     -= _systems[i].OnUpdate;
                LateUpdateSystems -= _systems[i].OnLateUpdate;
                StopAllSystems    -= _systems[i].OnStop;
            }

            DisposeAllSystems();

            _systems = null;
        }
    }
}