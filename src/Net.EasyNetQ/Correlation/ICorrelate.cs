namespace Net.EasyNetQ
{
    public interface ICorrelate 
    {
        object Identifier { get; }
    }
}