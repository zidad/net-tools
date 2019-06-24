using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;

namespace Net.EasyNetQ.Pipes
{
    public interface IPipe
    {
        void OnBeforeConsume<TMessage, TConsumer>(TConsumer consumer, TMessage message, CancellationToken cancellationToken = new CancellationToken())
            where TMessage : class
            where TConsumer : IConsume<TMessage>;

        void OnAfterConsume<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception, CancellationToken cancellationToken = new CancellationToken())
            where TMessage : class
            where TConsumer : IConsume<TMessage>;

        Task OnBeforeConsumeAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message, CancellationToken cancellationToken = new CancellationToken())
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>;

        Task OnAfterConsumeAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception, CancellationToken cancellationToken = new CancellationToken())
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>;
    }
}