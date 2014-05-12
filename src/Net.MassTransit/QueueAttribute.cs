using System;
using MassTransit.Util;

namespace Net.MassTransit.Meta
{
    /// <summary>
    /// Indicates whether a class that this attribute is applied to should run in the specified queue. 
    /// Defaults to the class name if the queue name is not specified.
    /// If it's applied to something else then a IConsumer or ISaga, it will indicate that any IServiceBus constructor parameter is resolved to the correct named instance of that bus
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true), MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    public class QueueAttribute : Attribute
    {
        private int? retryCount;
        public string Name { get; set; }

        public int? ConcurrentConsumerLimit { get; set; }

        public bool HighAvailable { get; set; }

        public bool Transient { get; set; }

        public int? RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; }
        }

        public int Retry
        {
            get { return retryCount ?? 1; }
            set { retryCount = value; }
        }

        public bool InheritorsRequireOwnQueue { get; set; }

        public QueueAttribute()
        {
        }

        public QueueAttribute(string name)
        {
            Name = name;
        }

        public QueueAttribute(int concurrentConsumerLimit)
        {
            ConcurrentConsumerLimit = concurrentConsumerLimit;
        }

        public bool Equals(QueueAttribute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as QueueAttribute);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public QueueSettings Settings
        {
            get
            {
                return new QueueSettings
                {
                    Name = Name,
                    ConcurrentConsumerLimit = ConcurrentConsumerLimit,
                    HighAvailable = HighAvailable,
                    InheritorsRequireOwnQueue = InheritorsRequireOwnQueue,
                    Transient = Transient,
                    RetryCount = RetryCount
                };
            }
        }
    }
}
