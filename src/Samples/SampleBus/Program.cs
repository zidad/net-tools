using Topshelf;

namespace SampleBus
{
    class Program
    {
        static int Main(string[] args)
        {
            var result = HostFactory.Run(x =>
            {
                x.Service<SampleBusService>();
                x.RunAsNetworkService();
                x.SetDescription(typeof(Program).Namespace);
                x.SetDisplayName(typeof(Program).Namespace);
                x.SetServiceName(typeof(Program).Namespace);
            });

            return (int)result;
        }
    }
}
