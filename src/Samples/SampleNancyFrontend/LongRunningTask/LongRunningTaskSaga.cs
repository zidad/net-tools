using System.Collections.Generic;
using EasyNetQ.AutoSubscribe;
using Net.Collections;
using Net.EasyNetQ;

namespace SampleNancyFrontend.LongRunningTask
{
    public class LongRunningTaskSaga : ISaga<LongRunningTaskSagaInstance>,
        IConsume<TaskStarted>,
        IConsume<TaskProgress>,
        IConsume<TaskFinished>
    {
        public LongRunningTaskSagaInstance Instance { get; set; }

        public void Consume(TaskStarted message)
        {
            Instance.State = TaskStatus.Running;
            Instance.Log.AddRange(message.Log);
        }

        public void Consume(TaskProgress message)
        {
            Instance.Progress = message.Progress;
            Instance.Log.AddRange(message.Log);
        }

        public void Consume(TaskFailed message)
        {
            Instance.State = TaskStatus.Error;
            Instance.Log.AddRange(message.Log);
        }

        public void Consume(TaskFinished message)
        {
            Instance.State = TaskStatus.Finished;
            Instance.Log.AddRange(message.Log);
        }
    }

    public class TaskFailed : TaskEvent
    {
    }

    public enum TaskStatus 
    {
        NotStarted,
        Running,
        Error,
        Finished
    }
    
    public class TaskEvent : ICorrelateBy<int>
    {
        public TaskEvent()
        {
            Log = new List<TaskLogEntry>();
        }

        public int Id { get; set; }
        
        public IList<TaskLogEntry> Log { get; set; }
    }

    public class TaskLogEntry
    {
        public TaskLogLevel Level { get; set; }
    }

    public enum TaskLogLevel
    {
        Debug,
        Information,
        Warning,
        Error,
    }

    public class TaskFinished : TaskEvent
    {

    }

    public class TaskProgress : TaskEvent
    {
        public int Progress { get; set; }
    }

    public class TaskStarted : TaskEvent
    {
    }

    public class LongRunningTaskSagaInstance : ICorrelateBy<int>, IFinishable
    {
        public int Id { get; set; }

        public TaskStatus State { get; set; }

        public int Progress { get; set; }

        public bool Finished 
        { 
            get { return State == TaskStatus.Finished; } 
        }
        
        public IList<TaskLogEntry> Log { get; set; }
        
    }
}
