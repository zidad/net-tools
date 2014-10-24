// MIT Licensed from http://github.com/zidad/net-tools

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.NonGeneric;
using Net.EasyNetQ;
using SampleMessages.LongRunningTasks;

namespace SampleBus
{
    public class SampleLongRunningTask  : IConsumeAsync<StartSampleTask>
    {
        private readonly IBus bus;
        private readonly IKeyGenerator<string> generator;
        private string taskInstanceId;

        public SampleLongRunningTask
        (
            IBus bus, 
            IKeyGenerator<string> generator
        )
        {
            this.bus = bus;
            this.generator = generator;
        }

        private void PublishTaskEvent(ICorrelateBy<string> taskEvent)
        {
            taskEvent.Id = taskInstanceId;
            bus.Publish(taskEvent.GetType(), taskEvent);
        }

        public Task Consume(StartSampleTask message)
        {
            taskInstanceId = generator.NewKey();

            PublishTaskEvent(new StartTask() );

            return Task
                .WhenAll(CreateTasks(message).ToArray())
                .ContinueWith(_ => PublishTaskEvent(new FinishTask()));
        }

        private IEnumerable<Task> CreateTasks(StartSampleTask message)
        {
            return Enumerable.Range(0, message.Amount)
                .Select(taskNumber => 
                    Task
                        .Delay(1000)
                        .ContinueWith(_ => PublishTaskEvent(new ProgressTask { Progress = taskNumber / message.Amount })));
        }
    }

}
