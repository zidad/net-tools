using System.Collections.Generic;
using Net.EasyNetQ;

namespace SampleNancyFrontend.LongRunningTask
{
    public class LongRunningTaskSagaInstance : ICorrelateBy<string>, IFinishable
    {
        public string Id { get; set; }

        public TaskStatus State { get; set; }

        public int Progress { get; set; }

        public bool Finished 
        { 
            get { return State == TaskStatus.Finished; } 
        }
        
        public IList<TaskLogEntry> Log { get; set; }
        
    }
}