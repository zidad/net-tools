using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyNetQ;
using Nancy;
using Net.EasyNetQ.Persistence;
using SampleNancyFrontend.LongRunningTask;

namespace SampleNancyFrontend.Modules
{
    public class TasksModule : NancyModule
    {
        private readonly IRepository<string, LongRunningTaskSagaInstance> tasks;
        private readonly IBus bus;

        public TasksModule(IRepository<string, LongRunningTaskSagaInstance> tasks, IBus bus) : base("api/tasks")
        {
            this.tasks = tasks;
            this.bus = bus;
            Get["/"] = x => Response.AsJson(tasks.GetAll());
            Post["/start"] = (parameter) =>
            {
                bus.Publish(new StartSampleTask());
                return new Response {StatusCode = HttpStatusCode.OK};
            };


        }
    }
}