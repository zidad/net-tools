using System;

namespace Net.EasyNetQ.ErrorHandling
{
    [Serializable]
    public class MessageRetryException : Exception
    {
        public MessageRetryException(string message, Exception innerException)
            :base(message, innerException)
        {
            
        }
    }
}