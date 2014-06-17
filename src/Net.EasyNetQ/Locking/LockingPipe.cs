using System;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Net.Annotations;
using Net.Reflection;
using Net.Text;

namespace Net.EasyNetQ.Locking
{
    [UsedImplicitly]
    public class LockingPipe : IPipe, IDisposable
    {
        private readonly ILocker locker;
        private IDisposable _lock;

        public LockingPipe ([NotNull] ILocker locker)
        {
            if (locker == null) throw new ArgumentNullException("locker");
            this.locker = locker;
        }

        private static object GetCorrelationIdentifier<TMessage, TConsumer>(TMessage message)
            where TMessage : class
        {
            Type concreteCorrelateByType;
            
            if (!typeof (TMessage).IsOfGenericType(typeof (ICorrelateBy<>), out concreteCorrelateByType))
                throw new NotSupportedException("{0} implements {1} but {2} doesn't implement {3}".FormatWith(
                    typeof (TConsumer), typeof (IConsumeLocked<>)));

            var correlationIdType = concreteCorrelateByType.GetGenericArguments()[0];
            var handler = (ICorrelationIdHandler) Activator.CreateInstance(typeof (CorrelationIdHandler<>).MakeGenericType(correlationIdType));
            return handler.Get(message);
        }

        public void OnBeforeConsume<TMessage, TConsumer>(TConsumer consumer, TMessage message) 
            where TMessage : class 
            where TConsumer : IConsume<TMessage>
        {
            var requiresLocking = typeof(TConsumer).IsOfGenericType(typeof(IConsumeLocked<>));
            if (!requiresLocking)
                return;

            _lock = locker.AcquireLock(GetCorrelationIdentifier<TMessage, TConsumer>(message));
        }

        public void OnAfterConsume<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception) 
            where TMessage : class 
            where TConsumer : IConsume<TMessage>
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_lock != null)
                _lock.Dispose();
        }

        public async Task OnBeforeConsumeAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            var requiresLocking = typeof(TConsumer).IsOfGenericType(typeof(IConsumeLocked<>));
            if (!requiresLocking)
                return;

            _lock = await locker.AcquireLockAsync(GetCorrelationIdentifier<TMessage, TConsumer>(message));
        }

        public Task OnAfterConsumeAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            if (_lock != null)
                _lock.Dispose();

            return Task.FromResult(0);
        }
    }
}