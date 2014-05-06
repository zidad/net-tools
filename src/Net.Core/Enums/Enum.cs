using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Net.Reflection;
using Net.System;

namespace Net.Enums
{
    public class Enum
    {
        public static TAttribute GetAttribute<TAttribute>(object enumValue)
            where TAttribute : Attribute
        {
            if (enumValue == null) return null;

            var members = enumValue.GetType().GetMember(enumValue.ToString());

            if (members.Length == 0) return null;

            return members[0].GetCustomAttribute<TAttribute>();
        }

        public static string GetShortName<TEnum>(TEnum? @enum) where TEnum : struct
        {
            if (@enum == null) return string.Empty;

            return GetShortName(@enum.Value);
        }

        public static string GetName<TEnum>(TEnum @enum) where TEnum : struct
        {
            return GetAttribute<DisplayAttribute>(@enum).Get(s => s.Name, string.Empty);
        }

        public static string GetShortName<TEnum>(TEnum @enum) where TEnum : struct
        {
            return GetAttribute<DisplayAttribute>(@enum).Get(s => s.ShortName, string.Empty);
        }

        public static string GetDescription<TEnum>(TEnum? @enum) where TEnum : struct
        {
            return GetAttribute<DisplayAttribute>(@enum).Get(s => s.Description);
        }

        public static string GetDescription<TEnum>(TEnum @enum) where TEnum : struct
        {
            return GetAttribute<DisplayAttribute>(@enum).Get(s => s.Description, string.Empty);
        }

        public static global::System.Enum ByShortName(Type enumType, string shortName)
        {
            var enumValue = (global::System.Enum)enumType.GetFields()
                .Where(e => e.GetAttribute<DisplayAttribute>().Get(s => s.ShortName) == shortName)
                .Select(c => c.GetValue(null))
                .FirstOrDefault();

            return enumValue;
        }

        public static TEnum Parse<TEnum>(string enumValue)
            where TEnum : struct
        {
            TEnum e;

            if (global::System.Enum.TryParse(enumValue, true, out e))
                return e;

            return default(TEnum);
        }
    }

    [Serializable]
    public class Enum<TEnum>
        where TEnum : struct
    {
        private readonly int value;
        private readonly string name;
        private readonly TEnum enumValue;

        public Enum(TEnum value)
        {
            enumValue = value;
            this.value = Convert.ToInt32(value);
            name = value.ToString();
        }

        public T GetAttribute<T>() where T : Attribute
        {
            return typeof(TEnum).GetMember(name)[0].GetCustomAttribute<T>();
        }

        public string Name
        {
            get { return name; }
        }

        public int Value
        {
            get { return value; }
        }

        public TEnum EnumValue
        {
            get { return enumValue; }
        }

        public static TEnum Parse(string name, bool ignoreCase = false)
        {
            return (TEnum)global::System.Enum.Parse(typeof(TEnum), name, ignoreCase);
        }

        public static bool TryParse(string name, out TEnum enumvalue)
        {
            EnumList<TEnum> list = EnumList<TEnum>.Instance;
            if (!list.Contains(name))
            {
                enumvalue = default(TEnum);
                return false;
            }
            enumvalue = list[name].EnumValue;
            return true;
        }

        public static IEnumerable<TEnum> GetValues()
        {
            return global::System.Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        public static TEnum ConvertFrom<T>(T other)
        {
            return Parse(other.ToString(), true);
        }

        public static TEnum? ByShortName(string code)
        {
            return ByAttribute<DisplayAttribute>(attribute => attribute != null && attribute.ShortName == code);
        }

        public static TEnum? ByAttribute<TAttribute>(Func<TAttribute, bool> predicate) where TAttribute : Attribute
        {
            return EnumList<TEnum>.Instance.FirstOrDefault(v => predicate(v.GetAttribute<TAttribute>())).Get(e => (TEnum?)e.EnumValue);
        }

        public static bool IsDefined(int i)
        {
            return global::System.Enum.IsDefined(typeof(TEnum), i);
        }
    }
}
