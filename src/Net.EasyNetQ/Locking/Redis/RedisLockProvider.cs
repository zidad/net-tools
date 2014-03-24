using System;
using System.Threading.Tasks;
using Net.EasyNetQ.Locking;

namespace Net.EasyNetQ
{
    public class RedisLockProvider : ILocker
    {
        public IDisposable AcquireLock(object identifier)
        {
            throw new NotImplementedException();
        }

        public Task<IDisposable> AcquireLockAsync(object identifier)
        {
            throw new NotImplementedException();
        }
    }
}
