using EasyNetQ.FluentConfiguration;

namespace Net.EasyNetQ.Subscribing
{
    public class AdvancedSubscriptionConfiguration : SubscriptionConfiguration, IAdvancedSubscriptionConfiguration
    {
        public bool Durable { get; set; }

        public AdvancedSubscriptionConfiguration() : base(50)
        {
            Durable = true;
        }

        IAdvancedSubscriptionConfiguration IAdvancedSubscriptionConfiguration.Durable(bool durable)
        {
            Durable = durable;
            return this;
        }
    }
}