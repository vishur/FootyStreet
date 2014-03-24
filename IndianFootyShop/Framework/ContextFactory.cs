//using Framework.Practices.Containers;
//using Microsoft.Practices.ServiceLocation;
//using Microsoft.Practices.Unity;
using System;
using System.ComponentModel;
using System.Data.Entity;

namespace Framework
{
    /// <summary>
    /// Factory for DbContext
    /// </summary>
    public static class ContextFactory
    {
        #region Methods

        /// <summary>
        /// Gets an Instance of a TDbContext.
        /// <para>When in a Unit of Work Connections will be shared,
        /// and the same instance will be return for each TEntity.</para>
        /// <para>Otherwise a new instance will be create on each call.</para>
        /// </summary>
        /// <returns>Will return Null if not found.</returns>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public static TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext, new()
        {
            var handler = UnitOfWorkManager.GetSharedInstance<ContextHandler<TDbContext>>(
                () => new InternalContextHandler<TDbContext>());
            return handler.GetContext().Value;
        }

        /// <summary>
        /// Gets an Instance of a ContextHandler for TDbContext.
        /// <para>When in a Unit of Work Connections will be shared,
        /// and the same instance will be return for each TEntity.</para>
        /// <para>Otherwise a new instance will be create on each call.</para>
        /// </summary>
        /// <returns>Will return Null if not found.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IContextHandler GetContextHandler<TDbContext>() where TDbContext : DbContext, new()
        {
            var handler = UnitOfWorkManager.GetSharedInstance<ContextHandler<TDbContext>>(
                () => new InternalContextHandler<TDbContext>());
            return handler;
        }

        #endregion Methods

    }
}