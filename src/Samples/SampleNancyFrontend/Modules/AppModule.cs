using Nancy;

namespace SampleNancyFrontend.Modules
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