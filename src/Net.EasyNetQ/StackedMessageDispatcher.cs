using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Net.Reflection;

namespace Net.EasyNetQ
{
    class StackedMessageDispatcher
    {
    }

    public class class StackedMessageDispatcher
    {

    }



    public class LockingMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly ILockProvider lockProvider;
        private readonly IAutoSubscriberMessageDispatcher messageDispatcher;

        public LockingMessageDispatcher(ILockProvider lockProvider, IAutoSubscriberMessageDispatcher messageDispatcher)
        {
            this.lockProvider = lockProvider;
            this.messageDispatcher = messageDispatcher;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message) where TMessage : class where TConsumer : IConsume<TMessage>
        {
            var correlate = message as ICorrelate;

            

            if(typeof(TConsumer).IsOfGenericType(typeof(IConsumeLocked<,>)))


            using(lockProvider.AcquireLock(message))
        }

        public Task DispatchAsync<TMessage, TConsumer>(TMessage message) where TMessage : class where TConsumer : IConsumeAsync<TMessage>
        {
            throw new NotImplementedException();
        }
    }

    public interface ILockProvider
    {
    }
}
