using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndianFootyShop
{
    public class UnityMvcServiceLocator : IServiceLocator
    {
        IUnityContainer _container;

        public UnityMvcServiceLocator(IUnityContainer container)
        {
            _container = container;
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return _container.ResolveAll<TService>();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.ResolveAll(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }

        public TService GetInstance<TService>(string key)
        {
            return (TService)Resolve(typeof(TService), key);
        }

        public object GetInstance(Type serviceType)
        {
            return Resolve(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return Resolve(serviceType, key);
        }

        public object GetService(Type serviceType)
        {
            return Resolve(serviceType);
        }

        public void Release(object instance)
        {
            _container.Teardown(instance);
        }

        private object Resolve(Type serviceType, string key = null)
        {
            try
            {
                if (!_container.IsRegistered(serviceType) && !_container.IsRegistered(serviceType, key))
                {
                    return null;
                }
                else
                {
                    return _container.Resolve(serviceType, key);
                }
            }
            catch (Exception ex)
            {
                // Logger.LogErrorData(ex);
                return null;
            }
        }
    }
}