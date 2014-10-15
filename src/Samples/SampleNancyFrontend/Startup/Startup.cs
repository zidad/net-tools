// MIT Licensed from http://github.com/zidad/net-tools
using Microsoft.Owin;
using Owin;
using SampleMessages;

[assembly: OwinStartup(typeof(Startup))]

namespace SampleMessages
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            app.UseNancy();
        }
    }
}

