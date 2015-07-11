﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Manufaktura.Controls.IoC
{
    public class ManufakturaResolver
    {
        private List<object> createdServices = new List<object>();

        public void AddService(object service)
        {
            createdServices.Add(service);
        }

        public void AddServices(params object[] services)
        {
            createdServices.AddRange(services);
        }

        public IEnumerable<T> ResolveAll<T>() where T:class
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(T)));
            foreach (var type in types)
            {
                var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                if (!constructors.Any()) yield return Activator.CreateInstance(type) as T;
                foreach (var constructor in constructors)
                {
                    object[] matchedParameters;
                    if (TryBindParameters(constructor, out matchedParameters))
                        yield return Activator.CreateInstance(type, matchedParameters) as T;
                }
            }
        }

        private bool TryBindParameters(ConstructorInfo constructor, out object[] matchedParameters)
        {
            var matchedParameterList = new List<object>();
            var constructorParameters = constructor.GetParameters();
            foreach (var parameter in constructorParameters)
            {
                var matchingService = createdServices.FirstOrDefault(cs => parameter.ParameterType == cs.GetType() ||
                    parameter.ParameterType.IsAssignableFrom(cs.GetType()));
                if (matchingService == null) break;
                matchedParameterList.Add(matchingService);
            }
            matchedParameters = matchedParameterList.ToArray();
            return constructorParameters.Length == matchedParameters.Length;
        }
    }
}