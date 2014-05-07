namespace Net.EasyNetQ.Locking
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
    
    /*public class CorrelationIdHandler<TCorrelationIdType> : ICorrelationIdHandler<TCorrelationIdType>
    {
        public object Get(object message)
        {
            return ((ICorrelateBy<TCorrelationIdType>) message).Id;
        }

        void ICorrelationIdHandler<TCorrelationIdType>.Set(object message, TCorrelationIdType value)
        {
            Set(message, value);
        }

        TCorrelationIdType ICorrelationIdHandler<TCorrelationIdType>.Get(object message)
        {
            return (TCorrelationIdType) Get(message);
        }

        public void Set(object message, object value)
        {
            ((ICorrelateBy<TCorrelationIdType>)message).Id = (TCorrelationIdType) value;
        }
    }*/
}