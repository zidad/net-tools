// MIT Licensed from http://github.com/zidad/net-tools
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Net.Autofac;
using Net.Autofac.CommandLine;
using Serilog;
using Serilog.Enrichers;

namespace SampleConsoleApplication
{
    public class SampleTaskRunner : IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;
        private IContainer Container { get; set; }

        private void RegisterContainer()
        {
            Serilog.Debugging.SelfLog.Out = Console.Out;

            var builder = new ContainerBuilder();

            builder.RegisterModule(new RegistrationModule().IncludeAssemblies());
            builder.RegisterType<DisplayCommandLineTasks>().AsSelf();
            builder.Register(x => CreateLogger());

            Container = builder.Build();
        }

        private static ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.With(new MachineNameEnricher())
                .Enrich.With(new ProcessIdEnricher())
                .Enrich.WithProperty("UserName", Environment.UserName)
                .WriteTo.ColoredConsole()
                .MinimumLevel.Information()
                .CreateLogger();
        }

        public void Dispose()
        {
            try
            {
                if (Container != null)
                    Container.Dispose();

                Container = null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private Task Run(CancellationToken cancellationToken)
        {
            RegisterContainer();

            Console.CancelKeyPress += (sender, args) =>
            {
                cancellationTokenSource.Cancel();
                args.Cancel = true;
            };

            cancellationToken.Register(() => Container.Resolve<ILogger>().Warning("Canceling"));

            return Container
                .Resolve<DisplayCommandLineTasks>()
                .Run(cancellationToken);
        }

        public void Run()
        {
            cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = cancellationTokenSource.Token;

            Task.WaitAll(Run(cancellationToken));
        }
    }
}