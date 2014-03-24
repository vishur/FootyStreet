using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    ///     Static factory for IRepository&lt;TEntity&gt; creation.
    /// </summary>
    public static class RepositoryFactory
    {
        #region Fields

        /// <summary>
        ///     The Container containing a global level of type to type activators.
        /// </summary>
        private static readonly IServiceLocator Container =
            new Container("Repositories");

        #endregion

        #region Public Properties

        /// <summary>
        ///     Determines if there is already an active UnitOfWork
        ///     for the current thread.  When active all repositories
        ///     requested will share a connection.
        /// </summary>
        /// <remarks>
        ///     Shares contexts base off of context Type.
        /// </remarks>
        public static bool IsSharing
        {
            get { return UnitOfWorkManager.IsSharing; }
        }

        #endregion Public Properties

        #region Methods

        /// <summary>
        ///     Begins an Unit Of Work on Current Thread.
        ///     <para>
        ///         Until the unit of work is disposed of, all new Instances on this thread
        ///         will share a connection.  Whenever any Disposable object is disposed
        ///         of, all objected under this unit of work will be disposed of.
        ///     </para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        ///     Occurs when the last unit of work was not completed.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public static UnitOfWork BeginUnitOfWork()
        {
            return UnitOfWorkManager.BeginUnitOfWork(false);
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
        public static UnitOfWork BeginUnitOfWork(bool saveOnCompletion)
        {
            return UnitOfWorkManager.BeginUnitOfWork(saveOnCompletion);
        }

        /// <summary>
        ///     Ends the Unit of work, and disposes of all
        ///     Connections used.
        /// </summary>
        public static void EndUnitOfWork()
        {
            UnitOfWorkManager.EndUnitOfWork(false);
        }


        /// <summary>
        ///     Ends the Unit of work, and disposes of all
        ///     Connections used.
        /// </summary>
        /// <param name="saveChanges">Saves any changes made.</param>
        public static void EndUnitOfWork(bool saveChanges)
        {
            UnitOfWorkManager.EndUnitOfWork(saveChanges);
        }

        /// <summary>
        ///     Gets another Repository and attempts to share the context with it.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRepository<TEntity> GetRepository<TEntity>(IContextHandler handler)
        {
            if (IsSharing || handler == null)
            {
                return GetRepository<TEntity>();
            }

            var otherRepository = Container.GetInstance<IRepository<TEntity>>();
            if (otherRepository != null)
            {
                var otherHandler = otherRepository as IContextHandler;
                if (otherHandler != null && handler.ContextType == otherHandler.ContextType)
                {
                    otherHandler.ShareContext(handler);
                }
            }
            return otherRepository;
        }

        /// <summary>
        ///     Gets an Instance of a IRepository.
        ///     <para>
        ///         When in a Unit of Work Connections will be shared,
        ///         and the same instance will be return for each TEntity.
        ///     </para>
        ///     <para>Otherwise a new instance will be create on each call.</para>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>Will return Null if not found.</returns>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public static IRepository<TEntity> GetRepository<TEntity>()
        {
            return UnitOfWorkManager.GetSharedInstance(() =>
            {
                return Container.GetInstance<IRepository<TEntity>>();
            });
        }

        /// <summary>
        ///     Gets a Repository based off of a type
        /// </summary>
        /// <param name="entityType">Entity Type to find</param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRepository GetRepository(Type entityType)
        {
            if (entityType.Assembly.IsDynamic)
            {
                entityType = entityType.BaseType;
            }
            Type repoType = typeof(IRepository<>).MakeGenericType(entityType);

            return UnitOfWorkManager.GetSharedInstance(
                () => Container.GetInstance(repoType), repoType) as IRepository;
        }

        /// <summary>
        ///     Gets another Repository and attempts to share the context with it.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRepository GetRepository(Type entityType, IContextHandler handler)
        {
            if (IsSharing || handler == null)
            {
                return GetRepository(entityType);
            }
            if (entityType.Assembly.IsDynamic)
            {
                entityType = entityType.BaseType;
            }
            Type repoType = typeof(IRepository<>).MakeGenericType(entityType);
            if (handler.GetType().GetInterfaces().Contains(repoType))
            {
                return handler as IRepository;
            }
            object otherRepository = Container.GetInstance(repoType);
            if (otherRepository != null)
            {
                var otherHandler = otherRepository as IContextHandler;
                if (otherHandler != null && handler.ContextType == otherHandler.ContextType)
                {
                    otherHandler.ShareContext(handler);
                }
            }
            return otherRepository as IRepository;
        }

        #endregion Methods
    }
}
