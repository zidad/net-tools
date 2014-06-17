using Net.EasyNetQ.Locking;

namespace Net.EasyNetQ
{
    public class CorrelationIdHandler<TCorrelationIdType> : ICorrelationIdHandler
    {
        public object Get(object message)
        {
            return ((ICorrelateBy<TCorrelationIdType>) message).Id;
        }

        public void Set(object message, object value)
        {
            ((ICorrelateBy<TCorrelationIdType>)message).Id = (TCorrelationIdType) value;
        }
    }
}