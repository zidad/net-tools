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
            var mutex = new Mutex(true, identifier.To<string>());
            mutex.WaitOne();
            return mutex;
        }

        public Task<IDisposable> AcquireLockAsync(object identifier)
        {
            return Task.Factory.StartNew(() => AcquireLock(identifier));
        }
    }
}