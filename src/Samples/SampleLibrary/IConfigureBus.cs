using System;
using System.Reflection;
using Autofac;
using Net.Autofac;

namespace SampleLibrary
{
    public interface IConfigureBus
    {
        IConfigureBus ContainerCreated(Action<IContainer> container);
        IConfigureBus ConfigureContainer(Action<ContainerBuilder> builder);
        IConfigureBus LoadConsumersFrom(params Assembly[] assemblies);
        IConfigureBus Name(string ns);
        IConfigureBus DisableBus(bool disable = true);
        IConfigureBus IsGuiInstance(bool isGuiInstance = true);
        IConfigureBus IsWebInstance(bool isWebInstance = true);
        IConfigureBus ConfigureModule(Action<RegistrationModule> module);
        IConfigureBus LoadConsumers(params Type[] consumerTypes);
        IConfigureBus EnableDebugging(bool isDebuggingEnabled = true);
    }
}