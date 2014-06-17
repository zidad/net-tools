using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;

namespace Net.EasyNetQ.Subscribing
{
    public class GenericAutoSubscriber
    {
        // TODO: subscribe to generic subscribers
        // TODO: multiple generic arguments
        // TODO: respect generic arguments
    }

    public class ScheduleMe<T> 
    {
 
    }

    public class TimeOut<T> 
    {
 
    }

    public class Scheduler<T> : IConsume<ScheduleMe<T>> 
    {
        public void Consume(ScheduleMe<T> message)
        {
            throw new NotImplementedException();
        }
    }
}
