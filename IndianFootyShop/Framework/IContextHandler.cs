using System;
using System.Data.Entity;

namespace Framework
{
    /// <summary>
    /// An object based ContextHandler interface
    /// </summary>
    public interface IContextHandler : IDisposable
    {
        /// <summary>
        /// Disposing Event
        /// </summary>
        event EventHandler Disposing;

        /// <summary>
        /// The Context
        /// </summary>
        DbContext Context { get; }

        /// <summary>
        /// The type of the Context
        /// </summary>
        Type ContextType { get; }

        /// <summary>
        /// Sets the context of this handler from the providing handler.
        /// This will share the connection between the two contexts
        /// </summary>
        /// <param name="providingHandler">The handler who is providing the context</param>
        /// <exception cref="ObjectDisposedException">Context handler has already been disposed.</exception>
        /// <exception cref="ObjectDisposedException">Unable to share context since providing handler is null.</exception>
        /// <exception cref="ArgumentNullException">Providing context handler has already been disposed.</exception>
        /// <exception cref="NotSupportedException">Unable to share contexts because they were of different types.</exception>
        void ShareContext(IContextHandler providingHandler);
    }
}