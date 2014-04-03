using System;
using System.Threading.Tasks;
using Autofac;
using Net.EasyNetQ;
using Net.EasyNetQ.Autofac;
using Net.EasyNetQ.Locking;
using Net.EasyNetQ.Persistence;
using NUnit.Framework;

namespace Net.Tests.EasyNetQ
{
    [TestFixture]
    public class LockingConsumerTests
    {
        IContainer Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MutexLockProvider>().As<ILocker>();
            builder.RegisterType<LockingMessageHook>().As<IMessageHook>();
            builder.RegisterType<TestConsumer>();
            
            Container = builder.Build();
        }

        [Test]
        public void TestLocking()
        {
            var dispatcher = new AutofacMessageDispatcher(Container);

            Guid correlationId = Guid.NewGuid();

            Parallel.For(0, 3, delegate(int i) 
            {
                Console.WriteLine("Started: " + i);
                dispatcher.Dispatch<TestMessage, TestConsumer>(new TestMessage { CorrelationId = correlationId });
                Console.WriteLine("Ended: " + i);
            });
        }
    }
}
