using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace Net.EasyNetQ
{
    public class Saga<TSagaInstance>
        where TSagaInstance
    {
        public TSagaInstance Instance { get; set; }    
        public IBus Bus { get; set; }    
    }

    public class SagaHost<TSaga>
        where TSaga : SagaInstanc
    {
        
    }
    
    public interface ILock<T, C>
        where T: ICorrelatedBy<C>
    {
        IDisposable AcquireLock(T message);
    }

    public interface IPersistence<T, C>  
        where T: ICorrelatedBy<C>
    {
        void Save(T message);
        void Delete(T message);
    }

    public interface ICorrelatedBy<T>
    {
        public T 
    }

    public interface ICorrelation
    {
    
    }

    public class Saga
    {
        public 
    }

    public class TestSaga : ISaga
    {
        public void Initialize(IBus bus)
        {
            
        }

    }
}
