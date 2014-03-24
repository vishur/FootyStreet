using System;
using System.Data.Entity;
using Framework;

namespace Framework
{
    /// <summary>
    /// Delays Loading of the DbContext
    /// </summary>
    /// <typeparam name="TDbContext">DbContext Type</typeparam>
    public abstract class LazyContextHandler<TDbContext> :
        ContextHandler<TDbContext>
            where TDbContext : DbContext, new()
    {
        #region All fields

        /// <summary>
        /// The Handler which will share the context when loaded.
        /// </summary>
        internal LazyContextHandler<TDbContext> ProvidingContext = null;

        /// <summary>
        /// Lock for creating the context.
        /// </summary>
        private readonly object ThreadLock = new object();

        #endregion All fields

        #region Constructors

        internal LazyContextHandler()
        {
        }

        #endregion Constructors

        #region Public and protected properties

        /// <summary>
        /// Disposal
        /// </summary>
        /// <param name="disposing">disposing</param>
        /// <remarks>Override this method to add disposal of any additional objects</remarks>
        protected override void Dispose(bool disposing)
        {
            if (Disposing != null)
            {
                Disposing(this, new EventArgs());
            }
            if (ProvidingContext != null && !ProvidingContext.IsDisposing)
            {
                ProvidingContext.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion Public and protected properties

        #region Events

        /// <summary>
        /// Shared disposal for un-initialized Contexts
        /// </summary>
        /// <remarks>
        /// DbContextHandler will shared disposal for all entities
        /// that have the some context.  But it is possible to be sharing
        /// without the same context with lazy loading.
        /// </remarks>
        private event EventHandler Disposing;

        #endregion Events

        #region Internal members

        /// <summary>
        /// Delayed Loaded Context
        /// </summary>
        internal override Disposable<TDbContext> GetContext()
        {
            if (base.GetContext() == null)
            {
                lock (ThreadLock)
                {
                    //Must Check context != null twice for thread safety
                    if (base.GetContext() == null)
                    {
                        if (ProvidingContext != null)
                        {
                            base.ShareContextCore(ProvidingContext);
                            ProvidingContext.Disposing -= ProviderDisposing;
                            ProvidingContext = null;
                        }
                        else
                        {
                            if (HasDisposed)
                            {
                                throw new ObjectDisposedException(
                                    "Context handler has already been disposed.");
                            }
                            SetNewUnSharedContext(new TDbContext());
                        }
                    }
                }
            }
            return base.GetContext();
        }

        /// <summary>
        /// A lazy loading of shared contexts.
        /// Will not create a context just to share it, but will
        /// point to the provider for when the context is needed.
        /// </summary>
        /// <param name="providingHandler">The handler to provide the context</param>
        internal override sealed void ShareContextCore(ContextHandler<TDbContext> providingHandler)
        {
            LazyContextHandler<TDbContext> lazy = providingHandler
                as LazyContextHandler<TDbContext>;

            if (lazy == null || lazy.context != null)  //context field Not Context Attribute
            {
                base.ShareContextCore(providingHandler);
                ProvidingContext = null;
            }
            else
            {
                //This block delays loading when Sharing a Context
                SetContext(null);
                if (ProvidingContext != null)
                {
                    ProvidingContext.Disposing -= ProviderDisposing;
                }
                ProvidingContext = lazy;
                lazy.Disposing += ProviderDisposing;
            }
        }

        #endregion Internal members

        #region Private Members

        /// <summary>
        /// Subscribes to the event of the Provider disposing
        /// before the context is shared
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProviderDisposing(object sender, EventArgs e)
        {
            if (sender == ProvidingContext)
            {
                Dispose();
            }
        }

        #endregion Private Members
    }
}