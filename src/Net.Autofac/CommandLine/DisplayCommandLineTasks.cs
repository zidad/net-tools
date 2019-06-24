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
        readonly ILifetimeScope container;
        readonly ILogger logger;
        readonly CommandLineReader commandLineReader;

        public DisplayCommandLineTasks(ILifetimeScope container, ILogger logger, CommandLineReader commandLineReader)
        {
            this.container = container;
            this.logger = logger;
            this.commandLineReader = commandLineReader;
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

                int taskIndex;
                try
                {
                    taskIndex = readLine.To<int>() - 1;
                    if (tasks.Length <= taskIndex || taskIndex < 0)
                    {
                        logger.Error("Invalid task number");
                        continue;
                    }
                }
                catch (FormatException e)
                {
                    logger.Error(e, "");
                    continue;
                }

                var type = tasks[taskIndex];

                using (var scope = container.BeginLifetimeScope("task"))
                {
                    var task = scope.Resolve(type);
                    try
                    {
                        await RunTask(cancellationToken, task);

                        logger.Information("Succesfully ran task {task}", task);
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
                Console.WriteLine();
            }
        }

        async Task RunTask(CancellationToken cancellationToken, object task)
        {
            Type concreteType;
            var parameterlessTask = task as ICommandLineTask;
            if (parameterlessTask != null)
                await parameterlessTask.Run(cancellationToken);
            else if (task.GetType().IsOfGenericType(typeof(ICommandLineTask<>), out concreteType))
            {
                var paramType = concreteType.GetGenericArguments()[0];
                var methodInfo = task.GetType().GetMethod("Run", new[] { paramType, typeof(CancellationToken) });
                var parameterInfo = methodInfo.GetParameters()[0];
                var parameterName = parameterInfo.GetCustomAttribute<DescriptionAttribute>()
                    .Get(d => d.Description, parameterInfo.Name);
                var parameters = commandLineReader.ReadObject(paramType, cancellationToken, parameterName);
                await (Task)methodInfo.Invoke(task, new[] { parameters, cancellationToken });
            }
        }
    }
}
