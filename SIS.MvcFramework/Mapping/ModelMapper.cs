namespace SIS.MvcFramework.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ModelMapper
    {
        public static TDestination ProjectTo<TDestination>(object origin)
        {

            var destinationInstance = (TDestination)Activator.CreateInstance(typeof(TDestination));

            foreach (var originProperty in origin.GetType().GetProperties())
            {
                var propertyName = originProperty.Name;
                var destinationProperty = destinationInstance.GetType().GetProperty(propertyName);
                var originPropertyValue = originProperty.GetValue(origin);

                if (destinationProperty != null)
                {
                    if (destinationProperty.PropertyType == typeof(string))
                    {
                        destinationProperty
                            .SetValue(destinationInstance, originPropertyValue.ToString());
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(destinationProperty.PropertyType))
                    {
                        var argumentType = destinationProperty.PropertyType.GetGenericArguments().FirstOrDefault();

                        var genericMethod = GetGenericMethod(argumentType);

                        var collection = typeof(List<>);
                        var condtructedCollection = collection.MakeGenericType(argumentType);
                        var destinationCollection = (IList)Activator.CreateInstance(condtructedCollection);

                        var orirginCollectionValue = (IEnumerable)originPropertyValue;

                        foreach (var item in orirginCollectionValue)
                        {
                            destinationCollection.Add(genericMethod.Invoke(typeof(ModelMapper), new object[] { item }));
                        }

                        destinationProperty.SetValue(destinationInstance, destinationCollection);
                    }
                    else if (destinationProperty.PropertyType.IsClass)
                    {
                        var argumentType = destinationProperty.PropertyType.GetGenericArguments().FirstOrDefault();

                        var genericMethod = GetGenericMethod(argumentType);

                        destinationProperty
                            .SetValue( destinationInstance, genericMethod.Invoke(typeof(ModelMapper), new object[] { originPropertyValue }));
                    }
                    else
                    {
                        destinationProperty
                            .SetValue(destinationInstance, originPropertyValue);
                    }
                }
            }

            return destinationInstance;
        }

        private static MethodInfo GetGenericMethod(Type argumentType)
        {
            var methodInfo = typeof(ModelMapper).GetMethod("ProjectTo");
            var genericMethod = methodInfo.MakeGenericMethod(argumentType);

            return genericMethod;
        }
    }
}
