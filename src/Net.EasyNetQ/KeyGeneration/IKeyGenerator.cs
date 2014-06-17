namespace Net.EasyNetQ
{
    public interface IKeyGenerator<out TKey>
    {
        TKey NewKey();
    }
}