// MIT Licensed from http://github.com/zidad/net-tools
using System.Threading;
using System.Threading.Tasks;
using Net.CommandLine;
using Serilog;

namespace SampleConsoleApplication
{
    public class SleepTask : ICommandLineTask<SleepParameters>
    {
        private readonly ILogger logger;

        public SleepTask(ILogger logger)
        {
            this.logger = logger;
        }

        public Task Run(SleepParameters sleep, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < sleep.Amount; i++)
                {
                    logger.Information("Start iteration: {count}", i);
                    Thread.Sleep(sleep.HowLong);
                    logger.Information("Finished iteration: {count}", i);

                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();
                }
            }, token);
        }
    }
}