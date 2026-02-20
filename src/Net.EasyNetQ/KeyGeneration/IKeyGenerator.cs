namespace Net.EasyNetQ.KeyGeneration
{
    public interface IKeyGenerator<out TKey>
    {
        TKey NewKey();
    }
}