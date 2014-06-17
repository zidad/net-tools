using System;
using EasyNetQ.AutoSubscribe;

namespace Net.EasyNetQ.Pipes
{
    public interface IErrorHandler
    {
        bool OnError<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception)
            where TMessage : class
            where TConsumer : IConsume<TMessage>;

        bool OnErrorAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>;
    }
}