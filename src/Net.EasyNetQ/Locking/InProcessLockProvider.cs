using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Net.Annotations;

namespace Net.EasyNetQ.Locking
{
    [UsedImplicitly]
    public class InProcessLockProvider : ILocker
    {
        private ConcurrentDictionary<object, object> _locks;

        public InProcessLockProvider()
        {
            _locks = new ConcurrentDictionary<object, object>();
        }

        public IDisposable AcquireLock(object identifier)
        {
            var lockObj = _locks.GetOrAdd(identifier, x => new object());
            var lockTaken = false;
            
            Monitor.TryEnter(lockObj, 1000, ref lockTaken);
            
            return new DisposeAction(()=> 
            {
                object val;
                if(_locks.TryRemove(identifier, out val))
                    Monitor.Exit(val);
            });
        }

        public Task<IDisposable> AcquireLockAsync(object identifier)
        {
            return Task.FromResult(AcquireLock(identifier));
        }
    }
}