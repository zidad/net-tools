using System;
using System.Threading;
using System.Threading.Tasks;
using Net.System;

namespace Net.EasyNetQ.Locking
{
    public class MutexLockProvider : ILocker
    {
        public IDisposable AcquireLock(object identifier)
        {
            bool createdNew;
            
            var mutex = new Mutex(true, identifier.To<string>(), out createdNew);
            
            if (!createdNew)
                mutex.WaitOne(2000);
            
            //return mutex;
            return new DisposableAction(()=> { mutex.ReleaseMutex(); });
        }

        public Task<IDisposable> AcquireLockAsync(object identifier)
        {
            return Task.Factory.StartNew(() => AcquireLock(identifier));
        }
    }

    public class DisposableAction : IDisposable
    {
        private readonly Action dispose;

        public DisposableAction(Action dispose)
        {
            this.dispose = dispose;
        }

        public void Dispose()
        {
            dispose();
        }
    }
}