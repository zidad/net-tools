using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Net.Reflection;
using Net.System;
using Net.Text;

namespace Net.CommandLine
{
    public static class CommandLineUtilities
    {
        static readonly Type[] primitiveTypes =
        { 
            typeof(string), 
            typeof(bool), 
            typeof(bool?), 
            typeof(int), 
            typeof(int?), 
            typeof(decimal), 
            typeof(decimal?), 
            typeof(long), 
            typeof(long?), 
            typeof(DateTime), 
            typeof(DateTime?), 
            typeof(Guid), 
            typeof(Guid?) 
        };

        public static T ReadObject<T>(CancellationToken cancellationToken)
        {
            return (T)ReadObject(typeof(T), cancellationToken);
        }

        public static object ReadObject(Type type, CancellationToken cancellationToken, string description = "")
        {
            // TODO: add support for enums, what to do with array types?
            // handle primitive types directly
            return primitiveTypes.Contains(type)
                ? ReadPrimitiveValue(type, description, cancellationToken)
                : ReadComplexValue(type, description, cancellationToken);
        }

        private static object ReadComplexValue(Type type, string description, CancellationToken cancellationToken)
        {
            var propertyInfos = type.GetProperties();
            object instance;
            try
            {
                instance = Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                throw new Exception("unable to instantiate type: " + type.FullName, e);
            }

            Console.WriteLine("Specify values for {0} [{1}]", description, type.Name);
            foreach (var propertyInfo in propertyInfos)
            {
                var defaultExpression = propertyInfo.GetValue(instance).To<string>().Wrap(" (", ")");
                var propertyDescription = propertyInfo.GetAttribute<DisplayNameAttribute>()
                        .Get(d => d.DisplayName, propertyInfo.Name) + defaultExpression;

                var value = ReadObject(propertyInfo.PropertyType, cancellationToken, propertyDescription);

                if (value == null)
                    continue; // keep default value

                propertyInfo.SetValue(instance, value);
            }

            return instance;
        }

        private static object ReadPrimitiveValue(Type propertyType, string propertyDescription, CancellationToken cancellationToken)
        {
            do
            {
                try
                {
                    Console.Write("Enter a value for {0}:", propertyDescription);
                    var line = Console.ReadLine();

                    if (line.HasNoValue())
                        return null;

                    return line.To(propertyType);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            while (!cancellationToken.IsCancellationRequested);

            return null;
        }
    }
}
