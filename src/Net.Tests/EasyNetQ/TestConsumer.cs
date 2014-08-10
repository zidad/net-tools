using System;
using System.Threading;
using EasyNetQ.AutoSubscribe;

namespace Net.EasyNetQ.Persistence
{
    public class TestConsumer : 
        ISaga<TestSagaInstance>, 
        IConsumeLocked<TestMessage>
    {
        public TestSagaInstance Instance { get; set; }
        
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