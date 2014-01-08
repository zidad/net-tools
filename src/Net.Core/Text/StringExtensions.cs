using System;
using System.Globalization;
using System.Text;

namespace Net.Text
{
    public static class StringExtensions
    {
        public static string FormatWith(this string source, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, source ?? string.Empty, args);
        }

        public static bool HasValue(this string data)
        {
            return !string.IsNullOrWhiteSpace(data);
        }

        // This method might seem redundant given that a HasValue method is also available. The reason it was introduced is to make it easier to read code that uses it.
        public static bool HasNoValue(this string data)
        {
            return string.IsNullOrWhiteSpace(data);
        }

        public static string ToPascalCase(this string s)
        {
            var result = ToCamelCase(s);
            return result[0].ToString().ToUpper() + result.Substring(1);
        }

        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            var sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                bool hasNext = (i + 1 < s.Length);
                if ((i == 0 || !hasNext) || char.IsUpper(s[i + 1]))
                {
                    sb.Append(char.ToLower(s[i], CultureInfo.InvariantCulture));
                }
                else
                {
                    sb.Append(s.Substring(i));
                    break;
                }
            }

            return sb.ToString();
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            return Contains(source, toCheck, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}