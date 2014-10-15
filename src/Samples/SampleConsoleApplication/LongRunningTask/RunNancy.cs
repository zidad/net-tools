// MIT Licensed from http://github.com/zidad/net-tools
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using Net.CommandLine;
using Net.Text;
using SampleMessages;

namespace SampleConsoleApplication.LongRunningTask
{
    public class StartNancy : ICommandLineTask<StartNancyParameters>
    {
        public Task Run(StartNancyParameters parameters, CancellationToken token)
        {
            Environment.CurrentDirectory = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName,
                ConfigurationManager.AppSettings["RootPath"]);

            using (var host = new NancyHost(new Uri("http://localhost:{0}/".FormatWith(parameters.Port)), new Bootstrapper { RootPath = Environment.CurrentDirectory }))
            {
                host.Start();
                Console.ReadLine();
                host.Stop();
            }

            return Task.FromResult(0);
        }
    }

    public class StartNancyParameters
    {
        public int Port { get; set; }
    }
}
