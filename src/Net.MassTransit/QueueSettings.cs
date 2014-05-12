using System;

namespace Net.MassTransit.Meta
{
    public class QueueSettings : IEquatable<QueueSettings>
    {
        public string Name { get; set; }

        public int? ConcurrentConsumerLimit { get; set; }

        public bool HighAvailable { get; set; }

        public bool Transient { get; set; }

        public int? RetryCount { get; set; }

        public bool InheritorsRequireOwnQueue { get; set; }

        public override string ToString()
        {
            return string.Format("ConcurrentConsumerLimit: {0}, HighAvailable: {1}, Name: {2}, Transient: {3}", ConcurrentConsumerLimit, HighAvailable, Name, Transient);
        }

        public bool Equals(QueueSettings other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((QueueSettings)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(QueueSettings left, QueueSettings right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(QueueSettings left, QueueSettings right)
        {
            return !Equals(left, right);
        }
    }
}