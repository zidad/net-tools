using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Net.CommandLine;
using Net.Reflection;
using Net.System;
using Net.Text;
using Serilog;

namespace Net.Autofac.CommandLine
{
    public class DisplayCommandLineTasks : ICommandLineTask
    {
        private readonly IComponentContext container;
        private readonly ILogger logger;

        public DisplayCommandLineTasks(IComponentContext container, ILogger logger)
        {
            this.container = container;
            this.logger = logger;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var tasks = container
                .ImplementationsFor<ICommandLineTask>()
                .Concat(container.ImplementationsForGenericType(typeof(ICommandLineTask<>)))
                .Where(t => t != GetType())
                .ToArray();

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var index = 1; index < tasks.Length + 1; index++)
                {
                    var taskType = tasks[index - 1];
                    logger.Information("{0}. {1}", index, taskType.Name);
                }

                logger.Information("Enter a number of a task or empty to exit");

                var readLine = Console.ReadLine();

                if (string.IsNullOrEmpty(readLine))
                    return;

                var type = tasks[readLine.To<int>() - 1];
                var task = container.Resolve(type);

                try
                {
                    Type concreteType;
                    if (task as ICommandLineTask != null)
                        await (task as ICommandLineTask).Run(cancellationToken);
                    else if (task.GetType().IsOfGenericType(typeof(ICommandLineTask<>), out concreteType))
                    {
                        var paramType = concreteType.GetGenericArguments()[0];
                        var methodInfo = task.GetType()
                            .GetMethod("Run", new Type[] { paramType, typeof(CancellationToken) });
                        var parameterInfo = methodInfo.GetParameters()[0];
                        var parameterName = parameterInfo.GetCustomAttribute<DescriptionAttribute>().Get(d => d.Description, parameterInfo.Name);
                        var parameters = CommandLineUtilities.ReadObject(paramType, parameterName);
                        await (Task)methodInfo.Invoke(task, new[] { parameters, cancellationToken });
                    }
                }
                catch (TaskCanceledException)
                {
                    logger.Warning("Task canceled by user {task}", task);
                }
                catch (OperationCanceledException)
                {
                    logger.Warning("Operation canceled by user {task}", task);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Failed to execute task {task}", task);
                }
            }
        }
    }
}
