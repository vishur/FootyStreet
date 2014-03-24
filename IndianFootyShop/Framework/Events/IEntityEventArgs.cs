using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Framework.Events
{
    /// <summary>
    /// Entity Event Args
    /// </summary>
    public interface IEntityEventArgs
    {
        /// <summary>
        /// The Current Property Values for the entity
        /// </summary>
        DbPropertyValues CurrentPropertyValues { get; }

        /// <summary>
        /// The Original Property Values for the entity
        /// </summary>
        DbPropertyValues OriginalPropertyValues { get; }

        /// <summary>
        /// The Db Entity Entry
        /// </summary>
        DbEntityEntry EntityEntry { get; }

        /// <summary>
        /// The Actual Entity
        /// </summary>
        object Entity { get; }

        /// <summary>
        /// They Entity Type
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// The Context Handler
        /// </summary>
        /// <returns></returns>
        IContextHandler GetContextHandler();
    }

    /// <summary>
    /// Entity Event Args
    /// </summary>
    public interface IEntityEventArgs<out TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// The Current Property Values for the entity
        /// </summary>
        DbPropertyValues CurrentPropertyValues { get; }

        /// <summary>
        /// The Original Property Values for the entity
        /// </summary>
        DbPropertyValues OriginalPropertyValues { get; }

        /// <summary>
        /// The Db Entity Entry
        /// </summary>
        DbEntityEntry EntityEntry { get; }

        /// <summary>
        /// The Actual Entity
        /// </summary>
        TEntity Entity { get; }

        /// <summary>
        /// The Actual Entity
        /// </summary>
        TEntity OriginalEntity { get; }

        /// <summary>
        /// The Context Handler
        /// </summary>
        /// <returns></returns>
        IContextHandler GetContextHandler();
    }
}