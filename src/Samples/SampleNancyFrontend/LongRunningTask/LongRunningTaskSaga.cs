using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Net.Collections;
using Net.EasyNetQ;

namespace SampleNancyFrontend.LongRunningTask
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

        public LongRunningTaskSagaInstance Instance { get; set; }

        public void Consume(StartTask message)
        {
            Instance.State = TaskStatus.Running;
            Instance.Log.AddRange(message.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), Instance.State, message.Log);

            Publish(message, new TaskStarted {});
        }

        private void Publish(TaskMessage source, TaskMessage message)
        {
            message.Id = source.Id;
            message.Log.AddRange(source.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), Instance.State, message.Log);

            bus.Publish(message);
        }

        public void Consume(ProgressTask message)
        {
            Instance.Progress = message.Progress;
            Instance.Log.AddRange(message.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), Instance.State, message.Log);
            
            Publish(message, new TaskProgress { });
        }

        public void Consume(FailTask message)
        {
            Instance.State = TaskStatus.Error;
            Instance.Log.AddRange(message.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), Instance.State, message.Log);
            
            Publish(message, new TaskFailed { });
        }

        public void Consume(FinishTask message)
        {
            Instance.State = TaskStatus.Finished;
            Instance.Log.AddRange(message.Log);

            logger.Information("Type: {Type}, State:{State}, Log: {Log}", message.GetType(), Instance.State, message.Log);
            
            Publish(message, new TaskFinished { });
        }
    }
}
