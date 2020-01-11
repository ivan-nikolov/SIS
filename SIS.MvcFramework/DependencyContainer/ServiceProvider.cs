using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SIS.MvcFramework.DependencyContainer
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly IDictionary<Type, Type> dependencyContainer;
        
        public ServiceProvider()
        {
            this.dependencyContainer = new ConcurrentDictionary<Type, Type>();
        }

        public void Add<TSource, TDestination>()
            where TDestination : TSource
        {
            this.dependencyContainer[typeof(TSource)] = typeof(TDestination);
        }

        public object CreateInstance(Type type)
        {
            if (this.dependencyContainer.ContainsKey(type))
            {
                type = this.dependencyContainer[type];
            }

            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(c => c.GetParameters().Count())
                .FirstOrDefault();

            if (constructor == null)
            {
                return null;
            }

            var constructorParameters = constructor.GetParameters();
            var parameterInstances = new List<object>();
            foreach (var parameter in constructorParameters)
            {
                var instance = CreateInstance(parameter.ParameterType);
                parameterInstances.Add(instance);
            }

            var obj = constructor.Invoke(parameterInstances.ToArray());

            return obj;
        }
    }
}
