using Framework;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Framework
{
    /// <summary>
    /// Design to share connections and manage disposal
    /// </summary>
    /// <typeparam name="TDbContext">DbContext Type</typeparam>
    public abstract class ContextHandler<TDbContext> : IDisposable, IContextHandler where TDbContext : DbContext
    {
        #region All fields

        /// <summary>
        /// Lock object for reading config
        /// </summary>
        private static readonly object readConfigLock = new object();

        /// <summary>
        /// The Isolation Level for Transactions
        /// </summary>
        private static IsolationLevel? contextIsolationLevel;

        /// <summary>
        /// If Isolation Level Set
        /// </summary>
        private static bool IsIsolationLevelSet = false;

        /// <summary>
        /// Is marked true when disposal has started.
        /// </summary>
        /// <remarks>
        /// Prevents the first ContextHandler having it's dispose method
        /// executed a second time by the Disposed Event.
        /// </remarks>
        private bool _startedDisposing = false;

        #endregion All fields

        #region Constructors

        /// <summary>
        /// Can only be inherited internally
        /// </summary>
        internal ContextHandler()
        {
        }

        #endregion Constructors

        #region Public and protected properties

        /// <summary>
        /// Gets the DbContext
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Object has been disposed of already.
        /// </exception>
        protected TDbContext Context
        {
            get
            {
                return GetContext().Value;
            }
        }

        #endregion Public and protected properties

        #region Methods

        /// <summary>
        /// Disposes of the DbContext, and any additional Handler
        /// that shares the the Context with this Handler
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            if (!HasDisposed && !_startedDisposing)
            {
                _startedDisposing = true;
                Dispose(true);
                HasDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Sets the context of this handler from the providing handler.
        /// This will share the connection between the two contexts
        /// </summary>
        /// <param name="providingHandler">The handler who is providing the context</param>
        /// <exception cref="ObjectDisposedException">Context handler has already been disposed.</exception>
        /// <exception cref="ObjectDisposedException">Unable to share context since providing handler is null.</exception>
        /// <exception cref="ArgumentNullException">Providing context handler has already been disposed.</exception>
        public void ShareContext(ContextHandler<TDbContext> providingHandler)
        {
            if (providingHandler != this)
            {
                if (HasDisposed)
                {
                    throw new ObjectDisposedException("Context handler has already been disposed.");
                }
                if (providingHandler == null)
                {
                    throw new ArgumentNullException("providingHandler",
                        "Unable to share context since providing handler is null.");
                }
                if (providingHandler.HasDisposed)
                {
                    throw new ObjectDisposedException("Providing context handler has already been disposed.");
                }

                ShareContextCore(providingHandler);
            }
        }

        /// <summary>
        /// Disposes of the DbContext, and any additional Handler
        /// that shares the Context with this Handler
        /// </summary>
        /// <param name="disposing">disposing</param>
        /// <remarks>Override this method to add disposal of any additional objects</remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (!HasDisposed && disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                }
                IContextHandler handlerInterface = this;
                if (this.Disposing != null)
                {
                    this.Disposing(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// This method is called when the Context has been updated
        /// </summary>
        /// <remarks>Designed for inheriting classes to override</remarks>
        protected virtual void UpdatedContext()
        {
        }

        /// <summary>
        /// This method is called when the Context is about to be updated
        /// </summary>
        /// <remarks>Designed for inheriting classes to override</remarks>
        protected virtual void UpdatingContext()
        {
        }

        /// <summary>
        /// Wraps an action delegate in a transaction scope.
        /// This is not straight forward to do otherwise.
        /// </summary>
        /// <param name="action">Action to perform in transaction</param>
        protected void WrapActionInTransaction(Action action)
        {
            DbConnection connection = Context.Database.Connection;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var scope = ContextIsolationLevel.HasValue ?
                Context.Database.Connection.BeginTransaction(ContextIsolationLevel.Value) : null)
            {
                bool success = false;
                try
                {
                    action();
                    success = true;
                }
                finally
                {
                    if (scope != null)
                    {
                        if (success)
                        {
                            scope.Commit();
                        }
                        else
                        {
                            scope.Rollback();
                        }
                    }
                }
            }
        }

        #endregion Methods

        #region Internal members

        /// <summary>
        /// The property that is the underlining context for
        /// First (virtual) GetContext and (sealed) SetContext
        /// Second property Context { get; }
        /// </summary>
        internal Disposable<TDbContext> context { get; private set; }

        /// <summary>
        /// Has already been disposed of
        /// </summary>
        internal bool HasDisposed { get; set; }

        /// <summary>
        /// Is currently being disposed
        /// </summary>
        internal bool IsDisposing
        {
            get
            {
                return context != null && context.IsDisposing;
            }
        }

        /// <summary>
        /// Returns the context.
        /// Used by the protected property Context
        /// </summary>
        /// <returns></returns>
        internal virtual Disposable<TDbContext> GetContext()
        {
            return context;
        }

        /// <summary>
        /// Sets the current Context
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>This seals the set and enables it to not be virtual</remarks>
        /// <exception cref="ArgumentNullException">Value is null.</exception>
        internal void SetContext(Disposable<TDbContext> value)
        {
            if (value != context)
            {
                UpdatingContext();
                if (context != null)
                {
                    context.Disposing -= SharedDisposing;
                    context.Disposed -= SharedDisposed;

                    //Removing dispose last event
                }
                context = value;
                if (value != null)
                {
                    //Entity Repo's force validation on save
                    context.Value.Configuration.ValidateOnSaveEnabled = false;

                    //We are a disconnected model.
                    context.Value.Configuration.AutoDetectChangesEnabled = false;
                    context.Disposing += SharedDisposing;
                    context.Disposed += SharedDisposed;

                    //Adding new dispose event
                }
                UpdatedContext();
            }
        }

        /// <summary>
        /// This is meant to be the first handler to have the context.
        /// If it is not the distributed dispose will not work.
        /// Use the ShareContext method for all other means.
        /// </summary>
        /// <param name="context">New context not in another Handler</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void SetNewUnSharedContext(TDbContext createContext)
        {
            if (createContext == null)
            {
                throw new ArgumentNullException("createContext", "Argument cannot be null.");
            }
            SetContext(new Disposable<TDbContext>(createContext));
        }

        /// <summary>
        /// This method is the default way of sharing a context.
        /// Designed for inheriting classes to override.
        /// </summary>
        /// <param name="providingHandler">The handler that provides the context to be shared.</param>
        internal virtual void ShareContextCore(ContextHandler<TDbContext> providingHandler)
        {
            var providedContext = providingHandler.GetContext();
            if (providedContext == null)
            {
                throw new InvalidOperationException(
                    "The providing handler does not have an instance of a context.");
            }
            SetContext(providedContext);
        }

        #endregion Internal members

        #region Private Members

        /// <summary>
        /// The Isolation Level for Transactions
        /// </summary>
        internal static IsolationLevel? ContextIsolationLevel
        {
            get
            {
                if (!IsIsolationLevelSet)
                {
                    lock (readConfigLock)
                    {
                        if (!IsIsolationLevelSet)
                        {
                            string configSectionName = String.Format(CultureInfo.InvariantCulture,
                                "{0}.IsolationLevel", typeof(TDbContext).Name);
                            string value = ConfigurationManager.AppSettings.Get(configSectionName);
                            if (String.IsNullOrWhiteSpace(value))
                            {
                                contextIsolationLevel = IsolationLevel.Serializable;
                            }
                            else if (value.ToLowerInvariant() != "none")  // Leave null for None
                            {
                                contextIsolationLevel = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), value);
                            }
                            IsIsolationLevelSet = true;
                        }
                    }
                }
                return contextIsolationLevel;
            }
        }

        /// <summary>
        /// This method subscribes to the event of Disposed by the context.
        /// </summary>
        /// <param name="sender">Disposable</param>
        /// <param name="args"></param>
        private void SharedDisposed(object sender, EventArgs args)
        {
            if (sender == context)
            {
                Dispose();
            }
        }

        /// <summary>
        /// This method subscribes to the event of Disposing by the context.
        /// </summary>
        /// <param name="sender">Disposable</param>
        /// <param name="e"></param>
        private void SharedDisposing(object sender, EventArgs e)
        {
            if (sender == context)
            {
                if (Disposing != null)
                {
                    Disposing(this, e);
                }
            }
        }

        #endregion Private Members

        #region Events

        /// <summary>
        /// Disposing Event
        /// </summary>
        internal EventHandler Disposing;

        #endregion Events

        #region Explicit Interface Methods

        /// <summary>
        /// Disposing Event
        /// </summary>
        event EventHandler IContextHandler.Disposing
        {
            add { this.Disposing += value; }
            remove { this.Disposing -= value; }
        }

        /// <summary>
        /// Returns the Context
        /// </summary>
        DbContext IContextHandler.Context
        {
            get { return this.Context; }
        }

        /// <summary>
        /// Returns the type of the Context
        /// </summary>
        Type IContextHandler.ContextType
        {
            get { return typeof(TDbContext); }
        }

        /// <summary>
        /// Tries to share the context from another context handler
        /// </summary>
        /// <param name="providingHandler">
        /// The handler providing the connection.
        /// </param>
        void IContextHandler.ShareContext(IContextHandler providingHandler)
        {
            ContextHandler<TDbContext> provider = providingHandler
                as ContextHandler<TDbContext>;
            if (provider != null)
            {
                this.ShareContext(provider);
            }
            else
            {
                throw new NotSupportedException(
                    "Unable to share contexts because they were of different types.");
            }
        }

        #endregion Explicit Interface Methods
    }
}