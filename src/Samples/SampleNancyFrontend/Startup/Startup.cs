using Microsoft.Owin;
using Owin;
using SampleNancyFrontend;

[assembly: OwinStartup(typeof(Startup))]

namespace SampleNancyFrontend
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

