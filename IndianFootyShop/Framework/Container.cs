using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System.Linq;

namespace Framework
{
    public class Container : IServiceLocator, IServiceRegister, IDisposable
    {
        internal readonly IUnityContainer UnityContainer;

        internal readonly LifetimeManager LifetimeManager = new ContainerControlledLifetimeManager();

        public Container()
        {
            UnityContainer = new UnityContainer();
        }

        public Container(IUnityContainer container)
        {
            this.UnityContainer = container;
        }

        public Container(string configuredContainerName)
        {
            UnityContainer = new UnityContainer();
            UnityContainer.LoadConfig(configuredContainerName);
        }

        public Container(string configuredContainerName, string sectionName)
        {
            UnityContainer = new UnityContainer();
            UnityContainer.LoadConfig(configuredContainerName, sectionName);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get. </param><filterpriority>2</filterpriority>
        public object GetService(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }

        /// <summary>
        /// Get an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param><exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is an error resolving
        ///             the service instance.</exception>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        public object GetInstance(Type serviceType)
        {
            return UnityContainer.ResolveNullable(serviceType);
        }

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param><param name="key">Name the object was registered with.</param><exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is an error resolving
        ///             the service instance.</exception>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        public object GetInstance(Type serviceType, string key)
        {
            return UnityContainer.ResolveNullable(serviceType, key);
        }

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        ///             registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param><exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is are errors resolving
        ///             the service instance.</exception>
        /// <returns>
        /// A sequence of instances of the requested <paramref name="serviceType"/>.
        /// </returns>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return UnityContainer.ResolveAllNullable(serviceType);
        }

        /// <summary>
        /// Get an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam><exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is are errors resolving
        ///             the service instance.</exception>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        public TService GetInstance<TService>()
        {
            return UnityContainer.ResolveNullable<TService>();
        }

        /// <summary>
        /// Get an instance of the given named <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam><param name="key">Name the object was registered with.</param><exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is are errors resolving
        ///             the service instance.</exception>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        public TService GetInstance<TService>(string key)
        {
            return UnityContainer.ResolveNullable<TService>();
        }

        /// <summary>
        /// Get all instances of the given <typeparamref name="TService"/> currently
        ///             registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam><exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is are errors resolving
        ///             the service instance.</exception>
        /// <returns>
        /// A sequence of instances of the requested <typeparamref name="TService"/>.
        /// </returns>
        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return UnityContainer.ResolveAllNullable<TService>();
        }

        public void RegisterMultipleInstances<T>(params T[] instances)
        {
            UnityContainer.RegisterMultipleInstances(instances);
        }

        public void RegisterMultipleInstances(Type type, params object[] instances)
        {
            UnityContainer.RegisterMultipleInstances(type, instances);
        }

        public void RegisterInstance(Type type, object instance)
        {
            UnityContainer.RegisterInstance(type, instance);
        }

        public void RegisterInstance<T>(T instance)
        {
            UnityContainer.RegisterInstance<T>(instance);
        }

        public void RegisterType(Type fromType, Type toType)
        {
            UnityContainer.RegisterType(fromType, toType);
        }

        public void Dispose()
        {
            UnityContainer.Dispose();
        }
    }
}
