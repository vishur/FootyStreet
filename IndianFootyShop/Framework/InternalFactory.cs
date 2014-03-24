using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Framework
{
    public abstract class InternalFactory
    {
        /// <summary>
        /// The Container containing a single thread level of type to instances.
        /// </summary>
        [ThreadStatic] private static UnityContainer _sharedInstances;

        internal static UnityContainer SharedInstances
        {
            get { return _sharedInstances; }
            set { _sharedInstances = value; }
        }

        /// <summary>
        /// Lock object
        /// </summary>
        private static readonly object Padlock = new object();

        /// <summary>
        /// The Container containing a global level of type to type activators.
        /// </summary>
        private static UnityContainer _container;

        /// <summary>
        /// flag to check the container is loaded or not
        /// </summary>
        private static bool _loaded;

        /// <summary>
        /// The objects in the thread is currently disposing
        /// </summary>
        [ThreadStatic]
        private static bool _threadIsDisposing;

        /// <summary>
        /// The thread level unit of work.
        /// </summary>
        [ThreadStatic]
        private static UnitOfWork _unitOfWork;

        /// <summary>
        /// Determines if there is already an active UnitOfWork
        /// for the current thread.  When active all repositories
        /// requested will share a connection.
        /// </summary>
        /// <remarks>
        /// Shares contexts base off of context Type.
        /// </remarks>
        public static bool IsSharing
        {
            get
            {
                return _unitOfWork != null && !_unitOfWork.HasDisposed;
            }
        }

        /// <summary>
        /// Begins an Unit Of Work on Current Thread.
        /// <para>
        /// Until the unit of work is disposed of, all new Instances on this thread
        /// will share a connection.  Whenever any Disposable object is disposed
        /// of, all objected under this unit of work will be disposed of.
        /// </para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// Occurs when the last unit of work was not completed.
        /// </exception>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Always)]
        public static UnitOfWork BeginUnitOfWork()
        {
            return BeginUnitOfWork(false);
        }

        /// <summary>
        /// Begins an Unit Of Work on Current Thread.
        /// <para>
        /// Until the unit of work is disposed of, all new Instances on this thread
        /// will share a connection.  Whenever any Disposable object is disposed
        /// of, all objected under this unit of work will be disposed of.
        /// </para>
        /// </summary>
        /// <param name="saveOnCompletion"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// Occurs when the last unit of work was not completed.
        /// </exception>
        public static UnitOfWork BeginUnitOfWork(bool saveOnCompletion)
        {
            if (!IsSharing)
            {
                _unitOfWork = new UnitOfWork(saveOnCompletion);
                _unitOfWork.Disposable.Disposing += DisposingEvent;
                _unitOfWork.SaveChanges += UnitOfWorkSaveChanges;

                //AllUnitsOfWork.Enqueue(UnitOfWork);
                SharedInstances = new UnityContainer();
                return _unitOfWork;
            }
            
            throw new InvalidOperationException(String.Format(
                Framework.Data.Properties.Resources.BeginUnitOfWorkException,
                _unitOfWork.StartingPoint));
        }

        /// <summary>
        /// Ends the Unit of work, and disposes of all
        /// Connections used.
        /// </summary>
        public static void EndUnitOfWork()
        {
            EndUnitOfWork(false);
        }

        /// <summary>
        /// Ends the Unit of work, and disposes of all
        /// Connections used.
        /// </summary>
        /// <param name="saveChanges">Saves any changes made.</param>
        public static void EndUnitOfWork(bool saveChanges)
        {
            if (IsSharing && !_threadIsDisposing)
            {
                _threadIsDisposing = true;

                _unitOfWork.Disposable.Disposing -= DisposingEvent;
                if (saveChanges || _unitOfWork.SaveOnCompletion)
                {
                    IEnumerable<ISave> saves = ResolveAllNullable<ISave>(SharedInstances);
                    foreach (ISave save in saves)
                    {
                        save.Save();
                    }
                }
                IEnumerable<IDisposable> disposables = ResolveAllNullable<IDisposable>(SharedInstances);
                foreach (IDisposable disposeable in disposables)
                {
                    disposeable.Dispose();
                }
                _unitOfWork.Dispose();
                _unitOfWork = null;
                SharedInstances.Dispose();
                SharedInstances = null;

                _threadIsDisposing = false;
            }
        }

        /// <summary>
        /// Creates a new instance of a based off of TEntity Type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        internal static IRepository<TEntity> GetNewInstance<TEntity>()
        {
            LoadConfiguration();

            IRepository<TEntity> result = ResolveNullable<IRepository<TEntity>>(_container);

            return result;
        }

        internal static object GetNewInstance(Type repositoryType)
        {
            LoadConfiguration();

            object result = ResolveNullable(_container, repositoryType);

            return result;
        }

        internal static void SetContextHandler(IContextHandler handler)
        {
            if (IsSharing)
            {
                Type dbContextType = handler.ContextType;
                Type handlerType = typeof(ContextHandler<>).MakeGenericType(dbContextType);

                IContextHandler sharedHandler = (IContextHandler)ResolveNullable(SharedInstances, handlerType);
                if (sharedHandler == null)
                {
                    SharedInstances.RegisterInstance(handlerType, handler);
                    RegisterMultipleInstance(SharedInstances, (ISave)handler);
                    RegisterMultipleInstance(SharedInstances, (IDisposable)handler);

                    handler.Disposing += DisposingEvent;
                }
                else if (!Equals(sharedHandler.Context, handler.Context))
                {
                    throw new InvalidOperationException
                        ("Unable to set local transaction with existing unit of work.");
                }
            }
        }

        /// <summary>
        /// Subscribes to Disposing Events of the UnitOfWork
        /// and ContextHandlers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void DisposingEvent(object sender, EventArgs e)
        {
            if (IsSharing &&
                (sender == _unitOfWork.Disposable ||
                 ResolveAllNullable<IDisposable>(SharedInstances).ToList().Contains(sender)))
            {
                EndUnitOfWork();
            }
            else if (sender is IContextHandler)
            {
                IContextHandler handler = sender as IContextHandler;
                List<IContextHandler> handlers = ResolveAllNullable<IContextHandler>(SharedInstances).ToList();
                if (handlers.Any(x => Equals(x.Context, handler.Context)))
                {
                    //Rare bug that happens.  This happens when the Framework creates the UnitOfWork.
                    EndUnitOfWork();
                }
            }
        }

        protected static void RegisterMultipleInstance<T>(UnityContainer container, T instance)
        {
            var list = ResolveAllNullable<T>(container);
            list.Add(instance);
        }

        protected static object ResolveNullable(UnityContainer containter, Type type)
        {
            try
            {
                return containter.Resolve(type);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        protected static T ResolveNullable<T>(UnityContainer containter)
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

        private static void LoadConfiguration()
        {
            if (!_loaded || _container == null)
            {
                lock (Padlock)
                {
                    if (!_loaded || _container == null)
                    {
                        _loaded = true;
                        _container = new UnityContainer();
                        UnityConfigurationSection section = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;
                        if (section != null) section.Configure(_container, "Repositories");
                    }
                }
            }
        }

        private static List<T> ResolveAllNullable<T>(UnityContainer container)
        {
            var list = ResolveNullable<List<T>>(container);
            if (list == null)
            {
                list = new List<T>();
                container.RegisterInstance(list);
            }
            return list;
        }

        /// <summary>
        /// Saves changes
        /// </summary>
        /// <param name="sender">Unit of work</param>
        /// <param name="e"></param>
        private static void UnitOfWorkSaveChanges(object sender, EventArgs e)
        {
            if (sender == _unitOfWork)
            {
                var saves = ResolveAllNullable<ISave>(SharedInstances).ToList();
                foreach (ISave save in saves)
                {
                    save.Save();
                }
            }
        }
    }
}