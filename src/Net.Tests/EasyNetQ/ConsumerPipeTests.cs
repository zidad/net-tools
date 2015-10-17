using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;

namespace Net.Tests.EasyNetQ
{
    public class ConsumerPipeTests
    {
        private class TestConsumer : IConsume<MessageA>
        {
            public void Consume(MessageA message)
            {
            }
        }
    }

    internal class MessageA
    {
    }
}
