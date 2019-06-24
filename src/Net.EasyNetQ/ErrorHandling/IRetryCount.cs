namespace Net.EasyNetQ.ErrorHandling
{
    public interface IRetryCount
    {
        int RetryCount { get; set; }
    }
}