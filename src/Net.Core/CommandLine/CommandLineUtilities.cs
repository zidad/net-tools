using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Net.System;
using Net.Text;

namespace Net.CommandLine
{
    public static class CommandLineUtilities
    {
        public static T ReadObject<T>()
        {
            return (T)ReadObject(typeof(T));
        }

        public static object ReadObject(Type type, string defaultName = "")
        {
            // handle primitive types directly
            if (new[] { typeof(string), typeof(int), typeof(decimal) }.Contains(type))
            {
                Console.WriteLine("Specify {0}", defaultName);
                return Console.ReadLine().To(type);
            }

            var propertyInfos = type.GetProperties();
            var instance = Activator.CreateInstance(type);

            Console.WriteLine("Specify {0} [{1}]", defaultName, type.Name);
            foreach (var propertyInfo in propertyInfos)
            {
                var isvalid = false;
                while (!isvalid)
                {
                    var defaultExpression = propertyInfo.GetValue(instance).To<string>().Wrap(" (", ")");

                    Console.Write("Enter a value for {0}.{1}{2}:", defaultName, propertyInfo.Name, defaultExpression);
                    var line = Console.ReadLine();

                    // TODO: parameter validation
                    isvalid = true;

                    if (line.HasNoValue())
                        continue;

                    propertyInfo.SetValue(instance, line.To(propertyInfo.PropertyType));
                }
            }

            return instance;
        }
    }
}
