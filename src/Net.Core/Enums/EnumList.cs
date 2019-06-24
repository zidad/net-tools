using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Net.Enums
{
    /// <summary>
    /// a bindable, double-indexed hashlist for any enum type
    /// </summary>
    [Serializable]
    public sealed class EnumList<TEnum> : KeyedCollection<string, Enum<TEnum>>
        where TEnum : struct
    {
        static volatile EnumList<TEnum> defaultInstance;
        
        // ReSharper disable once StaticFieldInGenericType
        static readonly object LockObj = new object();

        public static EnumList<TEnum> Instance
        {
            get
            {
                if (defaultInstance != null)
                    return defaultInstance;

                lock (LockObj)
                    if (defaultInstance == null)
                        defaultInstance = new EnumList<TEnum>(true);

                return defaultInstance;
            }
        }

        public EnumList(bool autoFill = false)
        {
            if (autoFill)
                FillList(typeof(TEnum));
        }

        public Enum<TEnum> this[TEnum enumValue]
        {
            get
            {
                return this.First(o => o.EnumValue.Equals(enumValue));
            }
        }

        public void Add(TEnum value)
        {
            Add(CreateEntry(value));
        }

        public void Add(params TEnum[] value)
        {
            foreach (var tEnum in value)
            {
                Add(CreateEntry(tEnum));
            }
        }

        public void Remove(TEnum value)
        {
            Remove(this[value]);
        }

        public static EnumList<T> Fill<T>(T enumType)
            where T : struct
        {
            return new EnumList<T>(true);
        }

        void FillList(Type enumType)
        {
            Clear();
            var valuesArray = global::System.Enum.GetValues(enumType);
            var entryCount = valuesArray.Length;
            var values = new TEnum[entryCount];
            valuesArray.CopyTo(values, 0);
            for (var i = 0; i < entryCount; i++)
            {
                Add(CreateEntry(values[i]));
            }
        }

        static Enum<TEnum> CreateEntry(TEnum value)
        {
            return new Enum<TEnum>(value);
        }

        protected override string GetKeyForItem(Enum<TEnum> item)
        {
            return item.Name;
        }
    }
}