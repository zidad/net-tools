using System.Threading.Tasks;

namespace Net.EasyNetQ
{
    public interface ICorrelatedStateHandler
    {
        void LoadState(object consumer, object message);
        void SaveState(object consumer, object message);
        Task LoadStateAsync(object consumer, object message);
        Task SaveStateAsync(object consumer, object message);
    }
}