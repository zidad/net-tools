using System;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Net.EasyNetQ.Autofac;
using Net.EasyNetQ.Pipes;

namespace Net.EasyNetQ.ErrorHandling
{
    public abstract class RetryStrategy : IErrorHandler
    {
        private int _maxRetries = 9;

        public IBus Bus { get; set; }

        public abstract TimeSpan CalculateDelay(int retries);

        public virtual int MaxRetries
        {
            get { return _maxRetries; }
            set { _maxRetries = value; }
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
                return false;
            }

            var delay = CalculateDelay(rc.RetryCount);

            // TODO: this should be a Send() to a specific queue
            Bus.FuturePublish(DateTime.Now + delay, message);

            return true;
        }
    }
}