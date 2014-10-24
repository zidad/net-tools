using Nancy;

namespace SampleMessages.Modules
{
    public class TicketModule : NancyModule
    {
        public TicketModule()
            : base("tickets")
        {
            Get["/"] = _ => View["app"];
        }
    }
}