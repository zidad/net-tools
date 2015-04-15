using System;

namespace Net.System
{
    /// <summary>
    /// An generic event args that can give back an extra typed value
    /// </summary>
    /// <typeparam name="T">The type of the value that should be given back</typeparam>
    public class EventArgs<T> : EventArgs
    {
        private readonly T value;

        public EventArgs(T value)
        {
            if (Equals(value, null))
                throw new Exception("Value of argument is not set");

            this.value = value;
        }

        public T Value
        {
            get { return value; }
        }
    }
}