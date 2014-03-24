namespace Net.EasyNetQ
{
    public interface ICorrelateBy<TIdentifier>
    {
        TIdentifier CorrelationId { get; set; }
    }
}