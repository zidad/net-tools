namespace Net.EasyNetQ
{
    public interface ISaga<TState>
    {
        TState Instance { get; set; }
    }

    public interface IFinishable
    {
        bool Finished { get; }
    }
}