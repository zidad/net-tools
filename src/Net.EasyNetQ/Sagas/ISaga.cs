namespace Net.EasyNetQ
{
    public interface ISaga<TState>
    {
        TState State { get; set; }
    }

    public interface IFinishable
    {
        bool Finished { get; }
    }
}