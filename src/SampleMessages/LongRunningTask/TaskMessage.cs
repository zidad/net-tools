using System.Collections.Generic;
using Net.EasyNetQ;

namespace SampleNancyFrontend.LongRunningTask
{
    public class TaskMessage : ICorrelateBy<string>
    {
        protected TaskMessage()
        {
            Log = new List<TaskLogEntry>();
        }

        public string Id { get; set; }
        
        public IList<TaskLogEntry> Log { get; set; }
    }
}