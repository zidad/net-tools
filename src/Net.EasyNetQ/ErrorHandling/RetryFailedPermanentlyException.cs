using System;

namespace Net.EasyNetQ.ErrorHandling
{
    [Serializable]
    public class RetryFailedPermanentlyException : Exception
    {
        public RetryFailedPermanentlyException(Exception innerException)
            : base("message failed permanently", innerException)
        {
        }
    }
}