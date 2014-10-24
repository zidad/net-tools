using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Net.Autofac;
using Net.Collections;

namespace SampleLibrary
{
    public class AutofacBusInstanceConfiguration : IConfigureBus
    {
        internal IList<Assembly> ConsumerAssemblies { get; private set; }
        internal IList<Type> ConsumerTypes { get; private set; }
        internal IList<Action<ContainerBuilder>> RegisterServices { get; private set; }
        internal IList<Action<IContainer>> ContainerCreatedActions { get; private set; }
        internal Action<RegistrationModule> ConfigureModule { get; private set; }
        internal string NamedInstance { get; set; }
        internal bool BusEnabled { get; set; }
        internal bool IsGuiInstance { get; set; }
        internal bool IsWebInstance { get; set; }
        public bool IsDebugging { get; set; }

        public AutofacBusInstanceConfiguration()
        {
            RegisterServices = new List<Action<ContainerBuilder>>();
            ContainerCreatedActions = new List<Action<IContainer>>();
            ConsumerAssemblies = new List<Assembly>();
            ConsumerTypes = new List<Type>();
            BusEnabled = true;
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                NamedInstance = entryAssembly.GetName().Name;
                ConsumerAssemblies.Add(entryAssembly);
            }
        }

        IConfigureBus IConfigureBus.ContainerCreated(Action<IContainer> container)
        {
            ContainerCreatedActions.Add(container);
            return this;
        }

        IConfigureBus IConfigureBus.ConfigureContainer(Action<ContainerBuilder> builder)
        {
            RegisterServices.Add(builder);
            return this;
        }

        IConfigureBus IConfigureBus.LoadConsumersFrom(params Assembly[] assemblies)
        {
            ConsumerAssemblies.AddRange(assemblies);
            return this;
        }

        IConfigureBus IConfigureBus.LoadConsumers(params Type[] consumerTypes)
        {
            ConsumerTypes.AddRange(consumerTypes);
            return this;
        }

        IConfigureBus IConfigureBus.Name(string name)
        {
            NamedInstance = name;
            return this;
        }

        IConfigureBus IConfigureBus.DisableBus(bool disable)
        {
            BusEnabled = !disable;
            return this;
        }

        IConfigureBus IConfigureBus.IsGuiInstance(bool isGuiInstance)
        {
            IsGuiInstance = isGuiInstance;
            return this;
        }

        IConfigureBus IConfigureBus.IsWebInstance(bool isWebInstance)
        {
            IsWebInstance = isWebInstance;
            return this;
        }

        IConfigureBus IConfigureBus.EnableDebugging(bool isDebuggingEnabled)
        {
            IsDebugging = isDebuggingEnabled;
            return this;
        }

        IConfigureBus IConfigureBus.ConfigureModule(Action<RegistrationModule> module)
        {
            ConfigureModule = module;
            return this;
        }
    }
}