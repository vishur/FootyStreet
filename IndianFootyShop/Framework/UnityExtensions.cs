using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public static class UnityExtensions
    {
        private const string DefaultContainerName = "unity";

        public static void LoadConfig(this IUnityContainer container, string configuredContainerName)
        {
            LoadConfig(container, configuredContainerName, DefaultContainerName);
        }

        public static void LoadConfig(this IUnityContainer container, string configuredContainerName, string sectionName)
        {
            if (String.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = DefaultContainerName;
            }
            var section = ConfigurationManager.GetSection(sectionName) as UnityConfigurationSection;
            if (section != null)
            {
                try
                {
                    section.Configure(container, configuredContainerName);
                }
                catch (ArgumentException e)
                {
                    if (!e.Message.Contains("not defined in this configuration section."))
                    {
                        throw;
                    }
                }

            }
        }

        public static void RegisterMultipleInstances<T>(this IUnityContainer container, params T[] instances)
        {
            var list = ResolveAllNullable<T>(container);
            list.AddRange(instances);
        }

        public static void RegisterMultipleInstances(this IUnityContainer container, Type type, params object[] instances)
        {
            var list = ResolveAllNullable(container, type) as IList;
            foreach (object instance in instances)
            {
                list.Add(instance);
            }
        }

        public static object ResolveNullable(this IUnityContainer container, Type type)
        {
            try
            {
                return container.Resolve(type);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public static object ResolveNullable(this IUnityContainer container, Type type, string name)
        {
            try
            {
                return container.Resolve(type, name);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }


        public static T ResolveNullable<T>(this IUnityContainer containter)
        {
            try
            {
                return containter.Resolve<T>();
            }
            catch (ResolutionFailedException)
            {
                return default(T);
            }
        }

        public static T ResolveNullable<T>(this IUnityContainer containter, string name)
        {
            try
            {
                return containter.Resolve<T>(name);
            }
            catch (ResolutionFailedException)
            {
                return default(T);
            }
        }



        public static List<T> ResolveAllNullable<T>(this IUnityContainer container)
        {
            var list = ResolveNullable<List<T>>(container);
            if (list == null)
            {
                list = new List<T>();
                container.RegisterInstance(list);
            }
            return list;
        }

        public static IEnumerable<object> ResolveAllNullable(this IUnityContainer container, Type type)
        {
            Type genericType = typeof(List<>).MakeGenericType(type);

            var list = ResolveNullable(container, genericType);
            if (list == null)
            {
                list = Activator.CreateInstance(genericType);
                container.RegisterInstance(list);
            }
            return list as IEnumerable<object>;
        }
    }
}
