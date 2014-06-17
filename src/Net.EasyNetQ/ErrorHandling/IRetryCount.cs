namespace Net.EasyNetQ.Autofac
{
    public interface IRetryCount
    {
        int RetryCount { get; set; }
    }
}