using System;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Net.Annotations;
using Net.Reflection;
using Net.Text;

namespace Net.EasyNetQ.Locking
{
    public class LockingMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly ILocker locker;
        private readonly IAutoSubscriberMessageDispatcher messageDispatcher;

        public LockingMessageDispatcher
        (
            [NotNull] ILocker locker,
            [NotNull] IAutoSubscriberMessageDispatcher messageDispatcher
        )
        {
            if (locker == null) throw new ArgumentNullException("locker");
            if (messageDispatcher == null) throw new ArgumentNullException("messageDispatcher");
            this.locker = locker;
            this.messageDispatcher = messageDispatcher;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message) where TMessage : class where TConsumer : IConsume<TMessage>
        {
            var requiresLocking = typeof(TConsumer).IsOfGenericType(typeof(IConsumeLocked<>));
            if (!requiresLocking)
            {
                messageDispatcher.Dispatch<TMessage, TConsumer>(message);
                return;
            }

            var correlatedMessage = message as ICorrelate;
            if (correlatedMessage == null)
                throw new NotSupportedException("{0} implements {1} but {2} doesn't implement {3}".FormatWith(typeof(TConsumer), typeof(IConsumeLocked<>)));

            using(locker.AcquireLock(correlatedMessage.Identifier))
                messageDispatcher.Dispatch<TMessage, TConsumer>(message);
        }

        public async Task DispatchAsync<TMessage, TConsumer>(TMessage message) where TMessage : class where TConsumer : IConsumeAsync<TMessage>
        {
            var requiresLocking = typeof(TConsumer).IsOfGenericType(typeof(IConsumeLocked<>));
            if (!requiresLocking)
            {
                await messageDispatcher.DispatchAsync<TMessage, TConsumer>(message);
                return;
            }

            var identifier = GetCorrelationIdentifier<TMessage, TConsumer>(message);

            using(await locker.AcquireLockAsync(identifier))
                await messageDispatcher.DispatchAsync<TMessage, TConsumer>(message);
        }

        private static object GetCorrelationIdentifier<TMessage, TConsumer>(TMessage message) where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            var correlatedMessage = message as ICorrelate;

            if (correlatedMessage == null)
                throw new NotSupportedException("{0} implements {1} but {2} doesn't implement {3}".FormatWith(
                    typeof (TConsumer), typeof (IConsumeLocked<>)));

            return correlatedMessage.Identifier;
        }
    }
}