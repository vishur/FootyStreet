using Framework.Data.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    internal static class UnitOfWorkManager
    {
        /// <summary>
        ///     The Container containing a single thread level of type to instances.
        /// </summary>
        [ThreadStatic]
        private static Dictionary<Type, object> sharedInstances;

        /// <summary>
        ///     The objects in the thread is currently disposing
        /// </summary>
        [ThreadStatic]
        private static bool threadIsDisposing;

        /// <summary>
        ///     The thread level unit of work.
        /// </summary>
        [ThreadStatic]
        private static UnitOfWork unitOfWork;

        private static Dictionary<Type, object> SharedInstances
        {
            get { return sharedInstances; }
            set { sharedInstances = value; }
        }

        /// <summary>
        ///     Determines if there is already an active UnitOfWork
        ///     for the current thread.  When active all repositories
        ///     requested will share a connection.
        /// </summary>
        /// <remarks>
        ///     Shares contexts base off of context Type.
        /// </remarks>
        internal static bool IsSharing
        {
            get { return unitOfWork != null && !unitOfWork.HasDisposed; }
        }

        /// <summary>
        ///     Begins an Unit Of Work on Current Thread.
        ///     <para>
        ///         Until the unit of work is disposed of, all new Instances on this thread
        ///         will share a connection.  Whenever any Disposable object is disposed
        ///         of, all objected under this unit of work will be disposed of.
        ///     </para>
        /// </summary>
        /// <param name="saveOnCompletion"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        ///     Occurs when the last unit of work was not completed.
        /// </exception>
        internal static UnitOfWork BeginUnitOfWork(bool saveOnCompletion)
        {
            if (!IsSharing)
            {
                unitOfWork = new UnitOfWork(saveOnCompletion);
                unitOfWork.Disposable.Disposing += DisposingEvent;
                unitOfWork.SaveChanges += UnitOfWorkSaveChanges;

                SharedInstances = new Dictionary<Type, object>();
                return unitOfWork;
            }

            throw new InvalidOperationException(String.Format(
                Resources.BeginUnitOfWorkException,
                unitOfWork.StartingPoint));
        }

        /// <summary>
        ///     Ends the Unit of work, and disposes of all
        ///     Connections used.
        /// </summary>
        /// <param name="saveChanges">Saves any changes made.</param>
        internal static void EndUnitOfWork(bool saveChanges)
        {
            if (!IsSharing || threadIsDisposing)
            {
                return;
            }

            threadIsDisposing = true;

            unitOfWork.Disposable.Disposing -= DisposingEvent;
            if (saveChanges || unitOfWork.SaveOnCompletion)
            {
                IEnumerable<ISave> saves = SharedInstances.GetMultiple(typeof(ISave)).Cast<ISave>();
                foreach (ISave save in saves)
                {
                    save.Save();
                }
            }
            IEnumerable<IDisposable> disposables = SharedInstances.GetMultiple(typeof(IDisposable)).Cast<IDisposable>();
            foreach (IDisposable disposeable in disposables)
            {
                disposeable.Dispose();
            }
            unitOfWork.Dispose();
            unitOfWork = null;
            SharedInstances.Clear();
            SharedInstances = null;

            threadIsDisposing = false;
        }

        internal static T GetSharedInstance<T>(Func<T> createNewInstance) where T : class
        {
            if (!IsSharing)
            {
                return createNewInstance();
            }
            object instance;
            if (SharedInstances.TryGetValue(typeof(T), out instance))
            {
                return (T)instance;
            }

            return CreateNewSharedInstance(createNewInstance);
        }

        internal static object GetSharedInstance(Func<object> createNewInstance, Type type)
        {
            if (!IsSharing)
            {
                return createNewInstance();
            }

            object instance;
            if (SharedInstances.TryGetValue(type, out instance))
            {
                return instance;
            }

            return CreateNewSharedInstance(createNewInstance, type);
        }

        private static object CreateNewSharedInstance(Func<object> createNewInstance, Type type)
        {
            var contextHandler = createNewInstance() as IContextHandler;

            if (contextHandler == null)
            {
                return null;
            }
            Type handlerType = GetHandlerType(contextHandler);

            object obj;
            if (SharedInstances.TryGetValue(handlerType, out obj))
            {
                var sharedHandler = (IContextHandler)obj;
                contextHandler.ShareContext(sharedHandler);
            }
            else
            {
                RegisterContextHandler(handlerType, contextHandler);
            }
            return contextHandler;
        }

        private static T CreateNewSharedInstance<T>(Func<T> createNewInstance) where T : class
        {
            T newInstance = createNewInstance();

            var contextHandler = newInstance as IContextHandler;

            if (newInstance == null)
            {
                return null;
            }
            Type handlerType = GetHandlerType(contextHandler);
            object obj;
            if (SharedInstances.TryGetValue(handlerType, out obj))
            {
                var sharedHandler = (IContextHandler)obj;
                contextHandler.ShareContext(sharedHandler);
            }
            else
            {
                RegisterContextHandler(handlerType, contextHandler);
            }
            return newInstance;
        }

        private static Type GetHandlerType(IContextHandler shared)
        {
            IContextHandler handler = shared;
            Type dbContextType = handler.ContextType;
            Type handlerType = typeof(ContextHandler<>).MakeGenericType(dbContextType);
            return handlerType;
        }

        private static void RegisterContextHandler(Type handlerType, IContextHandler shared)
        {
            SharedInstances.Add(handlerType, shared);
            SharedInstances.AddMultiple(typeof(ISave), shared);
            SharedInstances.AddMultiple(typeof(IDisposable), shared);
            shared.Disposing += DisposingEvent;
        }


        /// <summary>
        ///     Subscribes to Disposing Events of the UnitOfWork
        ///     and ContextHandlers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void DisposingEvent(object sender, EventArgs e)
        {
            if (IsSharing &&
                (sender == unitOfWork.Disposable ||
                 SharedInstances.GetMultiple(typeof(IDisposable)).Contains(sender)))
            {
                EndUnitOfWork(false);
            }
            else if (sender is IContextHandler)
            {
                var handler = sender as IContextHandler;
                var handlers = SharedInstances.GetMultiple(typeof(IContextHandler)).Cast<IContextHandler>();
                if (handlers.Any(x => Equals(x.Context, handler.Context)))
                {
                    //Rare bug that happens.  This happens when the Framework creates the UnitOfWork.
                    EndUnitOfWork(false);
                }
            }
        }

        private static void AddMultiple(this Dictionary<Type, object> dictionary, Type type, object obj)
        {
            var list = dictionary.GetMultiple(type);
            list.Add(obj);
        }


        private static List<object> GetMultiple(this Dictionary<Type, object> dictionary, Type type)
        {
            object list;
            if (dictionary.TryGetValue(type, out list))
            {
                return (List<object>)list;
            }
            List<object> newList = new List<object>();
            dictionary.Add(type, newList);
            return newList;
        }

        /// <summary>
        ///     Saves changes
        /// </summary>
        /// <param name="sender">Unit of work</param>
        /// <param name="e"></param>
        private static void UnitOfWorkSaveChanges(object sender, EventArgs e)
        {
            if (sender == unitOfWork)
            {
                var saves = SharedInstances.GetMultiple(typeof(ISave)).Cast<ISave>();
                foreach (ISave save in saves)
                {
                    save.Save();
                }
            }
        }
    }
}
