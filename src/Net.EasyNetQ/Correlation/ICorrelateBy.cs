namespace Net.EasyNetQ
{
    public interface ICorrelateBy<TIdentifier> //: ICorrelate
    {
        TIdentifier CorrelationId { get; set; }
    }
}