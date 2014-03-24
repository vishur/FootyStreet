using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Framework.Entities;

namespace Framework
{
    internal static class ReflectionHelper
    {
        private static readonly ObjectCache KeyMapping = new MemoryCache("NameKeyMappingCache");
        private static readonly object KeyMappingLock = new object();

        private static readonly ObjectCache Properties = new MemoryCache("NameForeignKeyMappingCache");
        private static readonly object PropertiesLock = new object();

        internal static PropertyInfo[] GetPropertyInfos(this Type entityType, Type attributeType)
        {
            string keyName = String.Format("{0} - Multi - {1}", entityType.FullName, attributeType.FullName);
            object cached = KeyMapping.Get(keyName);
            if (cached is PropertyInfo[])
            {
                return (PropertyInfo[])cached;
            }
            if (entityType.Assembly.IsDynamic)
            {
                entityType = entityType.BaseType;
            }
            lock (KeyMappingLock)
            {
                cached = KeyMapping.Get(keyName);
                if (cached is PropertyInfo[])
                {
                    return (PropertyInfo[])cached;
                }

                var keyProp = entityType.GetProperties().
                                         Where(p => Attribute.IsDefined(p, attributeType)).ToArray();
                /*if (!keyProp.Any())
                {
                    throw new InvalidOperationException
                        (String.Format(
                            "Please check the version of your EDMX's T4, or figure out why {0} doesn't have a any properties with {1} attribue.",
                            entityType.FullName,
                            attributeType.Name));
                }*/
                KeyMapping.Set(keyName, keyProp,
                               new CacheItemPolicy
                               {
                                   SlidingExpiration = TimeSpan.FromMinutes(15)
                               });
                return keyProp;
            }
        }

        internal static PropertyInfo GetPropertyInfo(this Type entityType, Type attributeType)
        {
            string keyName = String.Format("{0} - Single - {1}", entityType.FullName, attributeType.FullName);
            object cached = KeyMapping.Get(keyName);
            if (cached is PropertyInfo)
            {
                return (PropertyInfo)cached;
            }
            if (entityType.Assembly.IsDynamic)
            {
                entityType = entityType.BaseType;
            }
            lock (KeyMappingLock)
            {
                cached = KeyMapping.Get(keyName);
                if (cached is PropertyInfo)
                {
                    return (PropertyInfo)cached;
                }

                var keyProp = entityType.GetProperties().
                                         SingleOrDefault(p => Attribute.IsDefined(p, attributeType));
                /*if (keyProp == null)
                {
                    throw new InvalidOperationException
                        (String.Format(
                            "Please check the version of your EDMX's T4, or figure out why {0} doesn't have a single property with {1} attribue.",
                            entityType.FullName,
                            attributeType.Name));
                }*/
                if (keyProp == null)
                {
                    return null;
                }
                KeyMapping.Set(keyName, keyProp,
                               new CacheItemPolicy
                               {
                                   SlidingExpiration = TimeSpan.FromMinutes(15)
                               });
                return keyProp;
            }
        }

        internal static void Clone<TEntity>(TEntity from, TEntity to)
        {
            var properties = typeof(TEntity).GetProperties();
            foreach (var property in properties)
            {
                if (property.CanWrite && property.CanRead)
                {
                    object fromValue = property.GetValue(from);
                    property.SetValue(to, fromValue);
                }
            }
        }

        internal static object[] GetPropertyValues(this PropertyInfo[] properties, object obj)
        {
            object[] results = new object[properties.Length];

            for (int index = 0; index < properties.Length; index++)
            {
                results[index] = properties[index].GetValue(obj);
            }

            return results;
        }


        internal static Dictionary<string, PropertyInfo> GetKeyProperties(object keyEntity)
        {
            Type type = keyEntity.GetType();
            string keyName = type.FullName + "~Key";
            object cached = Properties.Get(keyName);
            if (cached is Dictionary<string, PropertyInfo>)
            {
                return (Dictionary<string, PropertyInfo>)cached;
            }
            if (type.Assembly.IsDynamic)
            {
                type = type.BaseType;
            }
            lock (PropertiesLock)
            {
                cached = Properties.Get(keyName);
                if (cached is Dictionary<string, PropertyInfo>)
                {
                    return (Dictionary<string, PropertyInfo>)cached;
                }
                var properties = (from property in type.GetProperties()
                                  from attribute in property.GetCustomAttributes(typeof(EntityKeyAttribute))
                                  let pk = attribute as EntityKeyAttribute
                                  where pk != null
                                  orderby pk.Order
                                  select property).ToDictionary(x => x.Name, x => x);

                /*if (properties.Count == 0)
                {
                    throw new InvalidOperationException
                        (String.Format("Please check the version of your EDMX's T4, or figure out why {0} doesn't have a single property with Key attribue.",
                            type.FullName));
                }*/
                Properties.Set(keyName, properties,
                    new CacheItemPolicy()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(15)
                    });
                return properties;
            }
        }

        internal static PropertyInfo[] GetForeignKeyProperties(IDateRange dateRange)
        {
            Type rangeType = dateRange.GetType();
            string keyName = rangeType.FullName + "~ForeignKey";
            object cached = Properties.Get(keyName);
            if (cached is PropertyInfo[])
            {
                return (PropertyInfo[])cached;
            }
            if (rangeType.Assembly.IsDynamic)
            {
                rangeType = rangeType.BaseType;
            }
            lock (PropertiesLock)
            {
                cached = Properties.Get(keyName);
                if (cached is PropertyInfo[])
                {
                    return (PropertyInfo[])cached;
                }

                var properties = (from property in rangeType.GetProperties()
                                  from attribute in property.GetCustomAttributes(typeof(EntityForeignKeyAttribute))
                                  let fk = attribute as EntityForeignKeyAttribute
                                  where fk != null
                                  where fk.Name == dateRange.GetKeyEntityName()
                                  orderby fk.Order
                                  select property).ToArray();

                /*if (properties.Length == 0)
                {
                    throw new InvalidOperationException
                        (String.Format("Please check the version of your EDMX's T4, or figure out why {0} doesn't have a single property with EntityForeignKey attribue.",
                            rangeType.FullName));
                }*/
                Properties.Set(keyName, properties,
                    new CacheItemPolicy()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(15)
                    });
                return properties;
            }
        }

        internal static Type GetEntityType(object obj)
        {
            return obj.GetType().Assembly.IsDynamic
                                ? obj.GetType().BaseType
                                : obj.GetType();
        }

    }
}
