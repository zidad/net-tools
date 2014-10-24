namespace Net.EasyNetQ.Autofac
{
    public interface IAutofacSagaConfigurator
    {
        IAutofacSagaConfigurator StoreInMemory();
    }
}