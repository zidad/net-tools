using System;
using System.Threading;
using Net.EasyNetQ;

namespace Net.Tests.EasyNetQ
{
    public class TestConsumer : 
        ISaga<TestSagaInstance>
    {
        public TestSagaInstance State { get; set; }
        
        public void Consume(TestMessage message)
        {
            Console.WriteLine("Executing: " + message.Id);
            Thread.Sleep(1000);
            Console.WriteLine("Executed: " + message.Id);
        }
    }

    public class TestSagaInstance : ICorrelateBy<int>
    {
        public int Id { get; set; }
    }
}