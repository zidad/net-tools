using System;
using System.Threading.Tasks;

namespace Net.EasyNetQ.Locking
{
    public interface ILocker
    {
        IDisposable AcquireLock(object identifier);
        Task<IDisposable> AcquireLockAsync(object identifier);
    }
}