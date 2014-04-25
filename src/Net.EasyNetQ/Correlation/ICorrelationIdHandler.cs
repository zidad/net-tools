namespace Net.EasyNetQ.Locking
{
    public interface ICorrelationIdHandler
    {
        object Get(object message);
        void Set(object message, object value);
    }
}