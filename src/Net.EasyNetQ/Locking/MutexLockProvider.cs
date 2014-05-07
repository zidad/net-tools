using System;
using System.Threading;
using System.Threading.Tasks;
using Net.Annotations;
using Net.System;

namespace Net.EasyNetQ.Locking
{
    [UsedImplicitly]
    public class MutexLockProvider : ILocker
    {
        private bool owned;
        private Mutex mutex;

        public IDisposable AcquireLock(object identifier)
        {
            do
            {
                mutex = new Mutex(true, identifier.To<string>(), out owned);
                mutex.WaitOne(2000);
            } while (!owned);

            return new DisposeAction(()=> mutex.ReleaseMutex());
        }

        public Task<IDisposable> AcquireLockAsync(object identifier)
        {
            return Task.FromResult(AcquireLock(identifier));
        }
    }

    public class DisposeAction : IDisposable
    {
        private readonly Action disposeAction;

        public DisposeAction(Action disposeAction)
        {
            this.disposeAction = disposeAction;
        }

        public void Dispose()
        {
            disposeAction();
        }
    }
}