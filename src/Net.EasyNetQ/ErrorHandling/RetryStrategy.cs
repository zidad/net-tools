using System;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Net.EasyNetQ.Autofac;
using Net.EasyNetQ.Pipes;
using Net.System;

namespace Net.EasyNetQ.ErrorHandling
{
    public abstract class RetryStrategy : IErrorHandler
    {
        private int maxRetries = 9;

        protected RetryStrategy(IBus bus, IEasyNetQLogger logger)
        {
            Bus = bus;
            Logger = logger;
        }

        private IBus Bus { get; set; }

        private IEasyNetQLogger Logger { get; set; }

        protected abstract TimeSpan CalculateDelay(int retries);

        protected virtual int MaxRetries
        {
            get { return maxRetries; }
            set { maxRetries = value; }
        }

        public bool OnError<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            return Retry(message, exception);
        }

        public bool OnErrorAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            return Retry(message, exception);
        }

        protected virtual bool Retry<TMessage>(TMessage message, Exception exception)
            where TMessage : class
        {
            var rc = message as IRetryCount;

            if (rc == null)
                return false;

            rc.RetryCount++;

            if (rc.RetryCount > MaxRetries)
            {
                Logger.ErrorWrite(new RetryFailedPermanentlyException(exception));
                return false;
            }

            var delay = CalculateDelay(rc.RetryCount);

            Logger.ErrorWrite(new MessageRetryException("message failed, retrying according to strategy in " + delay.ToPrettyFormat(), exception));

            // TODO: this should be a Send() to a specific queue
            Bus.FuturePublish(DateTime.Now + delay, message);

            return true;
        }
    }
}