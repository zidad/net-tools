namespace Net.EasyNetQ.Autofac
{
    public class MessageContext : IMessageContext
    {
        public MessageContext(object message)
        {
            Message = message;
        }

        public object Message { get; set; }
    }
}