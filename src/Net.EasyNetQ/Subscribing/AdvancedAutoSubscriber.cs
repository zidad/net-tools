using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.FluentConfiguration;
using EasyNetQ.Topology;
using Net.Annotations;

namespace Net.EasyNetQ.Subscribing
{
    /// <summary>
    /// this is a copy from the Autofac project that adds Transient and Autodelete queues to the possibilities
    /// </summary>
    public class AdvancedAutoSubscriber : AutoSubscriber
    {
        private readonly IConventions _conventions;
        private readonly IAdvancedBus _advancedBus;

        public delegate AdvancedAutoSubscriber Factory(string subscriptionIdPrefix);

        public Action<IAdvancedSubscriptionConfiguration, AutoSubscriberConsumerInfo> SubscriptionConfiguration { get; set; }

        public AdvancedAutoSubscriber(string subscriptionIdPrefix, IBus bus, IConventions conventions)
            : base(bus, subscriptionIdPrefix)
        {
            _conventions = conventions;
            _advancedBus = bus.Advanced;

            SubscriptionConfiguration = (configuration, subscriber) => { };
        }

        /// <summary>
        /// Registers all consumers in passed assembly. The actual Subscriber instances is
        /// is marked with <see cref="AutoSubscriberConsumerAttribute"/> with a custom SubscriptionId.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for consumers.</param>
        public override void Subscribe([NotNull] params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException("assemblies");

            var subscriptionInfos = GetSubscriptionInfos(assemblies.SelectMany(a => a.GetTypes()), typeof(IConsume<>));

            InvokeMethods(subscriptionInfos, DispatchMethodName, messageType => typeof(Action<>).MakeGenericType(messageType),
                (info, dispatchDelegate) => Subscribe(info.MessageType, CreateSubscriptionId(info), x => dispatchDelegate.DynamicInvoke(x), TopicInfo(info)));
        }

        /// <summary>
        /// Registers all async consumers in passed assembly. The actual Subscriber instances is
        /// is marked with <see cref="AutoSubscriberConsumerAttribute"/> with a custom SubscriptionId.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for consumers.</param>
        public override void SubscribeAsync([NotNull] params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException("assemblies");

            var subscriptionInfos = GetSubscriptionInfos(assemblies.SelectMany(a => a.GetTypes()), typeof(IConsumeAsync<>));
            Func<Type, Type> subscriberTypeFromMessageTypeDelegate = messageType => typeof(Func<,>).MakeGenericType(messageType, typeof(Task));

            InvokeMethods(subscriptionInfos, DispatchAsyncMethodName, subscriberTypeFromMessageTypeDelegate,
                (subscriptionInfo, dispatchDelegate) =>
                    SubscribeAsync(subscriptionInfo.MessageType, CreateSubscriptionId(subscriptionInfo),
                        x => (Task)dispatchDelegate.DynamicInvoke(x), TopicInfo(subscriptionInfo)));
        }

        public IDisposable Subscribe
        (
            [NotNull] Type type,
            [NotNull] string subscriptionId,
            [NotNull] Action<object> onMessage,
            [NotNull] Action<IAdvancedSubscriptionConfiguration> configure
        )
        {
            if (type == null) throw new ArgumentNullException("type");
            if (subscriptionId == null) throw new ArgumentNullException("subscriptionId");
            if (onMessage == null) throw new ArgumentNullException("onMessage");
            if (configure == null) throw new ArgumentNullException("configure");

            return SubscribeAsync(type, subscriptionId, msg =>
            {
                var tcs = new TaskCompletionSource<object>();
                try
                {
                    onMessage(msg);
                    tcs.SetResult(null);
                }
                catch (Exception exception)
                {
                    tcs.SetException(exception);
                }
                return tcs.Task;
            },
                configure);
        }

        public IDisposable SubscribeAsync
        (
            [NotNull] Type type,
            [NotNull] string subscriptionId,
            [NotNull] Func<object, Task> onMessage,
            [NotNull] Action<IAdvancedSubscriptionConfiguration> configure
        )
        {
            if (type == null) throw new ArgumentNullException("type");
            if (subscriptionId == null) throw new ArgumentNullException("subscriptionId");
            if (onMessage == null) throw new ArgumentNullException("onMessage");
            if (configure == null) throw new ArgumentNullException("configure");

            var configuration = new AdvancedSubscriptionConfiguration();
            configure(configuration);

            var queueName = _conventions.QueueNamingConvention(type, subscriptionId);
            var exchangeName = _conventions.ExchangeNamingConvention(type);

            var queue = _advancedBus.QueueDeclare(queueName, durable: configuration.Durable, autoDelete: configuration.AutoDelete);
            var exchange = _advancedBus.ExchangeDeclare(exchangeName, ExchangeType.Topic);

            foreach (var topic in configuration.Topics.AtLeastOneWithDefault("#"))
            {
                _advancedBus.Bind(exchange, queue, topic);
            }

            Func<IMessage<object>, MessageReceivedInfo, Task> oms = (message, messageRecievedInfo) => onMessage(message.Body);

            return _advancedBus.Consume(queue, x => x.Add(oms));
        }

        private Action<IAdvancedSubscriptionConfiguration> TopicInfo(AutoSubscriberConsumerInfo subscriptionInfo)
        {
            var topics = GetTopAttributeValues(subscriptionInfo);
            var allTopics = topics as string[] ?? topics.ToArray();
            if (allTopics.Count() != 0)
            {
                return GenerateConfigurationFromTopics(allTopics);
            }
            return configuration =>
            {
                configuration.WithTopic("#");
                SubscriptionConfiguration(configuration, subscriptionInfo);
            };
        }

        private static IEnumerable<string> GetTopAttributeValues(AutoSubscriberConsumerInfo subscriptionInfo)
        {
            var consumeMethod = ConsumeMethod(subscriptionInfo);
            object[] customAttributes = consumeMethod.GetCustomAttributes(typeof(ForTopicAttribute), true);
            return customAttributes
                .OfType<ForTopicAttribute>()
                .Select(a => a.Topic);
        }

        private static Action<ISubscriptionConfiguration> GenerateConfigurationFromTopics(IEnumerable<string> topics)
        {
            return configuration =>
            {
                foreach (var topic in topics)
                {
                    configuration.WithTopic(topic);
                }
            };
        }

        private static MethodInfo ConsumeMethod(AutoSubscriberConsumerInfo consumerInfo)
        {
            return consumerInfo.ConcreteType.GetMethod(ConsumeMethodName, new[] { consumerInfo.MessageType }) ??
                   GetExplicitlyDeclaredInterfaceMethod(consumerInfo.MessageType);
        }

        private static MethodInfo GetExplicitlyDeclaredInterfaceMethod(Type messageType)
        {
            var interfaceType = typeof(IConsume<>).MakeGenericType(messageType);
            return interfaceType.GetMethod(ConsumeMethodName);
        }

        protected void InvokeMethods(IEnumerable<KeyValuePair<Type, AutoSubscriberConsumerInfo[]>> subscriptionInfos, string dispatchName, Func<Type, Type> subscriberTypeFromMessageTypeDelegate, Action<AutoSubscriberConsumerInfo, Delegate> subscribeMethod)
        {
            foreach (var kv in subscriptionInfos)
            {
                foreach (var subscriptionInfo in kv.Value)
                {
                    var dispatchMethod =
                        AutoSubscriberMessageDispatcher.GetType()
                            .GetMethod(dispatchName, BindingFlags.Instance | BindingFlags.Public)
                            .MakeGenericMethod(subscriptionInfo.MessageType, subscriptionInfo.ConcreteType);

                    var dispatchDelegate = Delegate.CreateDelegate(subscriberTypeFromMessageTypeDelegate(subscriptionInfo.MessageType), AutoSubscriberMessageDispatcher, dispatchMethod);

                    subscribeMethod(subscriptionInfo, dispatchDelegate);
                }
            }
        }

        private string CreateSubscriptionId(AutoSubscriberConsumerInfo subscriptionInfo)
        {
            var subscriptionAttribute = GetSubscriptionAttribute(subscriptionInfo);
            return subscriptionAttribute != null ? subscriptionAttribute.SubscriptionId : GenerateSubscriptionId(subscriptionInfo);
        }
    }
}
