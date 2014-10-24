﻿using System;
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
        private readonly IComponentContext _container;
        private readonly ILogger _logger;

        public DisplayCommandLineTasks(IComponentContext container, ILogger logger)
        {
            this._container = container;
            this._logger = logger;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var tasks = _container
                .ImplementationsFor<ICommandLineTask>()
                .Concat(_container.ImplementationsForGenericType(typeof(ICommandLineTask<>)))
                .Where(t => t != GetType())
                .ToArray();

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var index = 1; index < tasks.Length + 1; index++)
                {
                    var taskType = tasks[index - 1];
                    _logger.Information("{0}. {1}", index, taskType.Name);
                }

                _logger.Information("Enter a number of a task or empty to exit");

                var readLine = Console.ReadLine();

                if (string.IsNullOrEmpty(readLine))
                    return;

                var taskIndex = readLine.To<int>() - 1;
                if (tasks.Length <= taskIndex || taskIndex < 0)
                {
                    _logger.Error("Invalid task number");
                    continue;
                }

                var type = tasks[taskIndex];
                var task = _container.Resolve(type);

                try
                {
                    Type concreteType;
                    if (task as ICommandLineTask != null)
                        await (task as ICommandLineTask).Run(cancellationToken);
                    else if (task.GetType().IsOfGenericType(typeof(ICommandLineTask<>), out concreteType))
                    {
                        var paramType = concreteType.GetGenericArguments()[0];
                        var methodInfo = task.GetType().GetMethod("Run", new[] { paramType, typeof(CancellationToken) });
                        var parameterInfo = methodInfo.GetParameters()[0];
                        var parameterName = parameterInfo.GetCustomAttribute<DescriptionAttribute>().Get(d => d.Description, parameterInfo.Name);
                        var parameters = CommandLineUtilities.ReadObject(paramType, cancellationToken, parameterName);
                        await (Task)methodInfo.Invoke(task, new[] { parameters, cancellationToken });
                    }
                }
                catch (TaskCanceledException)
                {
                    _logger.Warning("Task canceled by user {task}", task);
                }
                catch (OperationCanceledException)
                {
                    _logger.Warning("Operation canceled by user {task}", task);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute task {task}", task);
                }
            }
        }
    }
}
