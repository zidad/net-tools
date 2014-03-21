using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;

namespace Net.EasyNetQ
{
    public class Saga<TSagaInstance>
        where TSagaInstance
    {
        public TSagaInstance Instance { get; set; }    
        public IBus Bus { get; set; }    
    }

    public class SagaHost<TSaga>
        where TSaga : SagaInstance
    {
        
    }
    
    public interface IConsumeLocked<T, C> : IConsume<T> where T : class
    {
    }

    public interface IPersistence<T, C>  
        where T: ICorrelate
    {
        void Save(T message);
        void Delete(T message);
    }

    public interface ICorrelate 
    {
        object Identifier { get; }
    }

    public interface ICorrelateBy<out TIdentifier> : ICorrelate
    {
        new TIdentifier Identifier { get; }
    }

    public class TestMessage : ICorrelateBy<Guid>
    {
        object ICorrelate.Identifier
        {
            get { return Identifier; }
        }

        public Guid Identifier { get; private set; }
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
