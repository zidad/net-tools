namespace Net.EasyNetQ.Persistence
{
    public interface ISaga<TState>    
    {
        TState State { get; set; }
    }
}