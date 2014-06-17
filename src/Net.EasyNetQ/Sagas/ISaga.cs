namespace Net.EasyNetQ
{
    public interface ISaga<TState>
    {
        TState State { get; set; }
    }
}