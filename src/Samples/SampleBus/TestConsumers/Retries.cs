using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Net.EasyNetQ.Autofac;
using Net.EasyNetQ.ErrorHandling;
using Net.EasyNetQ.Pipes;
using Serilog;

namespace SampleBus.TestConsumers
{

    public class TestRetryEvent : IRetryCount
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}", Id, Name);
        }

        public int RetryCount { get; set; }
    }

    [ExponentiallyIncreasingRetryStrategy(9)]
    public class TestRetryConsumer : IConsume<TestRetryEvent>
    {
        private readonly ILogger _logger;

        public TestRetryConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public void Consume(TestRetryEvent message)
        {
            if (message.RetryCount < 5)
                throw new Exception("Booo!");

            _logger.Warning("Yay!");
        }
    }

    public class ExponentiallyIncreasingRetryStrategy : RetryStrategy
    {
        public override TimeSpan CalculateDelay(int retries)
        {
            return TimeSpan.FromSeconds(Math.Pow(3, retries));
        }
    }

    public class ExponentiallyIncreasingRetryStrategyAttribute : ErrorHandlerAttribute
    {
        public int MaxRetries { get; set; }

        public ExponentiallyIncreasingRetryStrategyAttribute(int maxRetries = 9)
            : base(typeof(ExponentiallyIncreasingRetryStrategy), 0)
        {
            MaxRetries = maxRetries;
        }

        public override IErrorHandler Initialize(IErrorHandler handler)
        {
            ((ExponentiallyIncreasingRetryStrategy)handler).MaxRetries = MaxRetries;
            return handler;
        }
    }
}
