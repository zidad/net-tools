using System;

namespace Net.EasyNetQ
{
    public class TestMessage : ICorrelateBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}