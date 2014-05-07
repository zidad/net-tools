namespace Net.EasyNetQ
{
    public interface ICorrelateBy<TIdentifier>
    {
        TIdentifier Id { get; set; }
    }
}