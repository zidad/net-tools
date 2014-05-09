using EasyNetQ.FluentConfiguration;

namespace Net.EasyNetQ.Subscribing
{
    public interface IAdvancedSubscriptionConfiguration : ISubscriptionConfiguration
    {
        /// <summary>
        /// Indicate if queues for this subscription are declared durable (default:true)
        /// </summary>
        IAdvancedSubscriptionConfiguration Durable(bool durable = true);
    }
}