using System;

namespace Framework
{
    /// <summary>
    /// A event based wrapper around a Disposable object.
    /// Should be in a different namespace.
    /// </summary>
    /// <typeparam name="TDisposable"></typeparam>
    public class Disposable<TDisposable> : IDisposable where TDisposable : IDisposable
    {
        #region All fields

        /// <summary>
        /// The private field for the public Property Value
        /// </summary>
        private TDisposable localValue;

        #endregion All fields

        #region Contructors

        /// <summary>
        /// Constructor providing Disposable to watch
        /// </summary>
        /// <param name="value">A new instance of a Disposable</param>
        /// <example>new Disposable(new Object())</example>
        public Disposable(TDisposable value)
        {
            Value = value;
        }

        #endregion Contructors

        #region Public and protected properties

        /// <summary>
        /// Has the underlining object already be Disposed of
        /// </summary>
        public bool HasDisposed { get; private set; }

        /// <summary>
        /// Is the underlining object currently Disposing
        /// </summary>
        public bool IsDisposing { get; private set; }

        /// <summary>
        /// The underling object being watched by this wrapper
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Object has been disposed of already.
        /// </exception>
        public TDisposable Value
        {
            get
            {
                if (HasDisposed)
                {
                    throw new ObjectDisposedException("Object has been disposed of already.");
                }
                return localValue;
            }
            private set
            {
                localValue = value;
            }
        }

        #endregion Public and protected properties

        #region Methods

        /// <summary>
        /// Event based Disposing
        /// </summary>
        /// <seealso cref="Disposing"/>
        /// <seealso cref="Disposed"/>
        public void Dispose()
        {
            if (!HasDisposed && !IsDisposing)
            {
                IsDisposing = true;
                if (Disposing != null)
                {
                    Disposing(this, new EventArgs());
                }
                Dispose(true);
                if (Disposed != null)
                {
                    Disposed(this, new EventArgs());
                }
                HasDisposed = true;
                IsDisposing = false;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Disposes the wrapped object
        /// </summary>
        /// <param name="disposing">disposing</param>
        /// <remarks>Override this method to add disposal of any additional objects</remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !HasDisposed)
            {
                if (localValue != null)
                {
                    localValue.Dispose();
                }
            }
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Raised prior to Disposal of Underling Object.
        /// </summary>
        /// <remarks>Do Not throw an exception from methods subscribed to this event</remarks>
        public event EventHandler Disposing;

        /// <summary>
        /// Raised after to Disposal of Underling Object.
        /// </summary>
        /// <remarks>Do Not throw an exception from methods subscribed to this event</remarks>
        public event EventHandler Disposed;

        #endregion Events
    }
}