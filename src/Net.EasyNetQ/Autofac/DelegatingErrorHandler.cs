using System;
using EasyNetQ.AutoSubscribe;
using Net.EasyNetQ.ErrorHandling;

namespace Net.EasyNetQ.Autofac
{
    public class DelegatingErrorHandler : IErrorHandler
    {
        public bool OnError<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            var errorHandler = consumer as IErrorHandler;
            if (errorHandler != null)
                return errorHandler.OnError(consumer, message, exception);
            return false;
        }

        public bool OnErrorAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            var errorHandler = consumer as IErrorHandler;
            if (errorHandler != null)
                return errorHandler.OnErrorAsync(consumer, message, exception);
            return false;
        }
    }
}