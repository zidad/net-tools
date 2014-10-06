﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cassette;
using EasyNetQ.AutoSubscribe;

namespace SampleNancyFrontend.LongRunningTask.SignalR
{
    public class LongRunningTaskClientUpdater 
        :   IConsume<TaskFailed>, 
            IConsume<TaskFinished>, 
            IConsume<TaskProgress>, 
            IConsume<TaskStarted>
    {
        private readonly LongRunningTaskHub hub;

        public LongRunningTaskClientUpdater(LongRunningTaskHub hub)
        {
            this.hub = hub;
        }
        
        public void Consume(TaskStarted message)
        {
            hub.Started(message);
        }

        public void Consume(TaskFailed message)
        {
            hub.Failed(message);
        }

        public void Consume(TaskProgress message)
        {
            hub.Progress(message);
        }

        public void Consume(TaskFinished message)
        {
            hub.Finished(message);
        }

    }
}