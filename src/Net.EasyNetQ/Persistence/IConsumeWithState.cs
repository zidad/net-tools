namespace Net.EasyNetQ.Persistence
{
    public interface IConsumeWithState<TState>    
    {
        TState State { get; set; }
    }
}