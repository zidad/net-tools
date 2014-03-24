using EasyNetQ.AutoSubscribe;

namespace Net.EasyNetQ
{
    public interface IConsumeLocked<in TMessage> 
        : IConsume<TMessage> where TMessage : class
    {
    }
}