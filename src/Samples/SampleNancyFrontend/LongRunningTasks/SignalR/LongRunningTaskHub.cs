// MIT Licensed from http://github.com/zidad/net-tools

using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using SampleMessages.LongRunningTasks;

namespace SampleMessages.LongRunningTasks.SignalR
{
    public class LongRunningTaskHub : Hub
    {
        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public void Started(TaskStarted message)
        {
            Clients.All.taskStatusChanged(message.Id, "Running");
        }

        public void Failed(TaskFailed message)
        {
            Clients.All.taskStatusChanged(message.Id, "Failed");
        }

        public void Progress(TaskProgress message)
        {
            Clients.All.taskStatusChanged(message.Id, "Running");
        }

        public void Finished(TaskFinished message)
        {
            Clients.All.taskStatusChanged(message.Id, "Finished");
        }
    }
}