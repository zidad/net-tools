using System;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Net.Annotations;

namespace Net.EasyNetQ
{
    public interface IPipe
    {
        void OnBeforeConsume<TMessage, TConsumer>(TConsumer consumer, TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>;

        void OnAfterConsume<TMessage, TConsumer>(TConsumer consumer, TMessage message, [CanBeNull] Exception exception)
            where TMessage : class
            where TConsumer : IConsume<TMessage>;

        Task OnBeforeConsumeAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>;

        Task OnAfterConsumeAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message, [CanBeNull] Exception exception)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>;
    }
}