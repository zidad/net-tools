using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;

namespace Net.EasyNetQ
{
    public interface IMessageDispatcherHook
    {
        void OnBeforeConsume<TMessage, TConsumer>(IConsume<TMessage> consumer, TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>;

        void OnAfterConsume<TMessage, TConsumer>(IConsume<TMessage> consumer, TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>;

        Task OnBeforeConsumeAsync<TMessage, TConsumer>(IConsumeAsync<TMessage> consumer, TMessage message)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>;

        Task OnAfterConsumeAsync<TMessage, TConsumer>(IConsumeAsync<TMessage> consumer, TMessage message)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>;
    }
}