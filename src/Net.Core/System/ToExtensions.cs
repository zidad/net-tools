using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Net.Text;

namespace Net.System
{
    public static class ToExtensions
    {
        public static T To<T>(this object source, object defaultValue = null, string format = null, object errorValue = null, string errorExceptionMessage = null, Func<T, T> transform = null)
        {
            var value = (T)To(source, typeof(T), defaultValue, format, errorValue, errorExceptionMessage);

            if (transform != null) value = transform(value);

            return value;
        }

        public static object To(this object source, Type type, object defaultValue = null, string format = null, object errorValue = null, string errorExceptionMessage = null)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var value = ToInternal(source, type, format, errorValue, errorExceptionMessage);

            if (value == null && defaultValue != null)
                return defaultValue;

            return value;
        }

        public static T[] ToArray<T>(this IEnumerable source)
        {
            return (from object variable in source select variable.To<T>()).ToArray();
        }

        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable source)
        {
            return (from object variable in source select variable.To<T>());
        }

        static object ToInternal(object source, Type type, string format, object errorValue, string errorExceptionMessage)
        {
            try
            {
                if (source != null && type.IsInstanceOfType(source) && format == null)
                    return source;

                if (type == typeof(string))
                {
                    if (source == null)
                        return string.Empty;

                    if (format.HasNoValue())
                        return source.ToString().Trim();

                    if (source is DateTime || source is DateTime?)
                        return ((DateTime)source).ToString(format);

                    if (source is int || source is int?)
                    {
                        if (format == "MMM")
                            return CultureInfo.CurrentUICulture.DateTimeFormat.GetAbbreviatedMonthName((int)source);

                        return ((int)source).ToString(format);
                    }

                    if (source is decimal || source is decimal?)
                        return ((decimal)source).ToString(format);

                    switch (format.ToLowerInvariant())
                    {
                        case "camelcase":
                            return source.ToString().ToCamelCase();
                        case "pascalcase":
                            return source.ToString().ToPascalCase();
                    }

                    // Implement format support for different types here as necessary. 
                    throw new NotImplementedException("Conversion with format from type {0} to string is not supported".FormatWith(type));
                }

                if (type == typeof(TimeSpan))
                    return ToTimeSpan(Convert.ToString(source, CultureInfo.CurrentCulture));

                if (type == typeof(DateTime))
                    return ToDateTime(Convert.ToString(source, CultureInfo.CurrentCulture), format);

                if (type == typeof(int))
                    return ToInt(source);

                if (type == typeof(double))
                    return ToDouble(Convert.ToString(source, CultureInfo.CurrentCulture));

                if (type == typeof(decimal))
                    return ToDecimal(Convert.ToString(source, CultureInfo.CurrentCulture));

                if (type == typeof(Guid))
                    return (source == null) ? Guid.Empty : new Guid(source.ToString());

                if (type == typeof(bool))
                    return ToBoolean(source);

                if (typeof(Enum).IsAssignableFrom(type))
                    return ToEnum(source, type);

                //  Maybe we can convert all nullable types with another pass through To(source, Nullable.GetUnderlyingType(type))
                var targetTypeIsNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

                if (targetTypeIsNullable)
                {
                    if (source == null || (source is string && ((string)source).HasNoValue()))
                        return null;

                    if (source == DBNull.Value)
                        return null;

                    return ToInternal(source, Nullable.GetUnderlyingType(type), format, errorValue, errorExceptionMessage);
                }

                return Convert.ChangeType(source, type, CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                if (errorValue != null)
                    return errorValue;

                throw new FormatException(
                    errorExceptionMessage ??
                    "Cannot convert value '{0}' from {1} to {2}. See InnerException for more details.".FormatWith(source, source == null ? "null" : source.GetType().ToString(), type),
                    ex);
            }
        }

        static object ToEnum(object source, Type target)
        {
            if (source is string)
                return Enum.Parse(target, source.ToString(), true);

            return Enum.ToObject(target, source);
        }

        static object ToBoolean(object source)
        {
            if (source == null || source == DBNull.Value)
                return false;

            var s = source.ToString().ToLower().Trim();

            if (s == "1" || s == "j" || s == "ja" || s == "y" || s == "yes" || s == "true" || s == "t")
                return true;

            if (s == "0" || s == "n" || s == "nee" || s == "no" || s == "false" || s == string.Empty || s == "f")
                return false;

            return bool.Parse(s);
        }

        static object ToTimeSpan(string source)
        {
            return TimeSpan.Parse(source, CultureInfo.CurrentCulture);
        }

        static object ToDateTime(string source, string format)
        {
            if (source.HasNoValue())
                return DateTime.MinValue;

            int numericDate; //ignored, but need it for the out param of TryParse

            if (int.TryParse(source, out numericDate))
            {
                // if we want to make this culture-aware we could consider looking at CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern to see if the user has entered MMdd or ddMM. 
                // since we're only using dutch now I've hard-coded the pattern. Note that if you do change this you'll probably also need to change logic in rob.validate.js
                if (source.Length == 1 || source.Length == 2)
                    return DateTime.ParseExact(source.To<int>().ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year, "ddMMyyyy", CultureInfo.CurrentCulture);
                if (source.Length == 4)
                    return DateTime.ParseExact(source + DateTime.Now.Year, "ddMMyyyy", CultureInfo.CurrentCulture);
                if (source.Length == 6)
                    return DateTime.ParseExact(source, "ddMMyy", CultureInfo.CurrentCulture);
                if (source.Length == 8)
                    return DateTime.ParseExact(source, "ddMMyyyy", CultureInfo.CurrentCulture);

                throw new InvalidOperationException("Cannot parse {0} into a valid DateTime".FormatWith(source));
            }

            if (format.HasValue())
                return DateTime.ParseExact(source, format, CultureInfo.InvariantCulture);

            return DateTime.Parse(source, CultureInfo.CurrentCulture);
        }

        static object ToInt(object source)
        {
            if (source == null || source.ToString().HasNoValue())
                return 0;
            return Convert.ToInt32(source);
        }

        static object ToDouble(string source)
        {
            return source.HasNoValue() ? 0 : double.Parse(source, CultureInfo.CurrentCulture);
        }

        static object ToDecimal(string source)
        {
            return source.HasNoValue() ? 0 : decimal.Parse(source, CultureInfo.CurrentCulture);
        }

        public static TProperty Get<T, TProperty>(this T o, Func<T, TProperty> propertyGetter, TProperty defaultValue = default(TProperty))
            where T : class
        {
            return o == null ? defaultValue : propertyGetter(o);
        }

        public static TProperty Get<T, TProperty>(this T o, Func<bool> condition, Func<T, TProperty> propertyGetter, TProperty defaultValue = default(TProperty))
            where T : class
        {
            return condition() ? defaultValue : propertyGetter(o);
        }

        public static TProperty SelectValue<T, TProperty>(this T? o, Func<T, TProperty> propertyGetter, TProperty defaultValue = default(TProperty)) where T : struct
        {
            return !o.HasValue ? defaultValue : propertyGetter(o.Value);
        }

        public static TProperty SelectValue<T, TProperty>(this T? o, Func<T, TProperty> propertyGetter, Func<TProperty> defaultValueSelector) where T : struct
        {
            return !o.HasValue ? defaultValueSelector() : propertyGetter(o.Value);
        }
    }
}
