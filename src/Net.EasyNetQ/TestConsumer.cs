using System;

namespace Net.EasyNetQ.Persistence
{
    public class TestConsumer : 
        IConsumeWithState<TestSagaInstance>//, 
        //IConsumeLocked<TestMessage>
    {
        public TestSagaInstance State { get; set; }
        
        public void Consume(TestMessage message)
        {
            throw new NotImplementedException();
        }
    }

    public class TestSagaInstance : ICorrelateBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}