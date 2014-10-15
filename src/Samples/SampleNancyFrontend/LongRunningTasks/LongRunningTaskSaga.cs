// MIT Licensed from http://github.com/zidad/net-tools

using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Net.Collections;
using Net.EasyNetQ;
using SampleMessages.LongRunningTasks;

namespace SampleMessages.LongRunningTasks
{
    public class LongRunningTaskSaga : 
        ISaga<LongRunningTaskSagaInstance>,
        IConsume<StartTask>,
        IConsume<ProgressTask>,
        IConsume<FailTask>,
        IConsume<FinishTask>
    {
        private readonly Serilog.ILogger logger;
        private readonly IBus bus;

        public LongRunningTaskSaga(Serilog.ILogger logger, IBus bus)
        {
            this.logger = logger;
            this.bus = bus;
        }

        public LongRunningTaskSagaInstance State { get; set; }

        public void Consume(StartTask message)
        {
            State.State = TaskStatus.Running;
            State.Log.AddRange(message.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), State.State, message.Log);

            Publish(message, new TaskStarted {});
        }

        private void Publish(TaskMessage source, TaskMessage message)
        {
            message.Id = source.Id;
            message.Log.AddRange(source.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), State.State, message.Log);

            bus.Publish(message);
        }

        public void Consume(ProgressTask message)
        {
            State.Progress = message.Progress;
            State.Log.AddRange(message.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), State.State, message.Log);
            
            Publish(message, new TaskProgress { });
        }

        public void Consume(FailTask message)
        {
            State.State = TaskStatus.Error;
            State.Log.AddRange(message.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), State.State, message.Log);
            
            Publish(message, new TaskFailed { });
        }

        public void Consume(FinishTask message)
        {
            State.State = TaskStatus.Finished;
            State.Log.AddRange(message.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), State.State, message.Log);
            
            Publish(message, new TaskFinished { });
        }
    }
}
