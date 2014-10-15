// MIT Licensed from http://github.com/zidad/net-tools
using Nancy;

namespace SampleMessages.Modules
{
    public class AppModule : NancyModule
    {
        public AppModule()
            : base("")
        {

            Get["/"] = _ => Response.AsRedirect("~/app");

            Get["/app"] = _ => View["app"];
        }
    }
}