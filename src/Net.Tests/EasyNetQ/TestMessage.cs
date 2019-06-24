using System;
using Net.EasyNetQ;

namespace Net.Tests.EasyNetQ
{
    public class TestMessage : ICorrelateBy<Guid>
    {
        public Guid Id { get; set; }
    }
}