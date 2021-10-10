
using System;

namespace SLE.Systems
{
    using Unity.Jobs;

    public abstract class SystemBase : IDisposable
    {
        protected static readonly int workerThreadCount;
        protected static int GetBatchCount(int length)
        {
            int count = length / workerThreadCount;

            return count < 1 ? 1 : count;
        }

        static SystemBase()
        {
            workerThreadCount = Environment.ProcessorCount - 1;
        }
        ~SystemBase()
        {
            Dispose(false);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        { 
            return handle;
        }
        public virtual void OnUpdate(float time, float deltaTime) 
        {            
        }
        public virtual void OnLateUpdate(float time, float deltaTime)
        {
        }
        public virtual void OnStop()
        {
        }
        
        public static implicit operator bool(SystemBase lhs)
        {
            return lhs != null;
        }
    }
}