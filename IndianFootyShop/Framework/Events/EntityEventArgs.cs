using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Framework.Events
{
    /// <summary>
    /// Entity Event Arguments
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext</typeparam>
    /// <typeparam name="TEntity">The Entity type</typeparam>
    public sealed class EntityEventArgs<TDbContext, TEntity> : EventArgs, IEntityEventArgs, IEntityEventArgs<TEntity>
        where TDbContext : DbContext, new()
        where TEntity : class, new()
    {
        private readonly EntityRepository<TDbContext, TEntity> repository;

        /// <summary>
        /// The Context Handler
        /// </summary>
        public ContextHandler<TDbContext> ContextHandler
        {
            get { return repository; }
        }

        /// <summary>
        /// The current Entity
        /// </summary>
        public TEntity Entity { get; private set; }


        private TEntity _orignalEntity;
        public TEntity OriginalEntity 
        { 
            get { return _orignalEntity ?? (_orignalEntity = OriginalPropertyValues.ToObject() as TEntity); }
        }

        /// <summary>
        /// The Original Property Values for the entity
        /// </summary>
        public DbPropertyValues OriginalPropertyValues
        {
            get { return repository.OriginalPropertyValues(Entity); }
        }

        /// <summary>
        /// The Current Property Values for the entity
        /// </summary>
        public DbPropertyValues CurrentPropertyValues
        {
            get { return ContextHandler.GetContext().Value.Entry(Entity).CurrentValues; }
        }

        internal EntityEventArgs(EntityRepository<TDbContext, TEntity> repository, TEntity entity)
        {
            this.repository = repository;
            Entity = entity;
        }

        /// <summary>
        /// They Entity Type
        /// </summary>
        Type IEntityEventArgs.EntityType
        {
            get { return typeof (TEntity); }
        }

        /// <summary>
        /// The Actual Entity
        /// </summary>
        object IEntityEventArgs.Entity
        {
            get { return Entity; }
        }

        IContextHandler IEntityEventArgs.GetContextHandler()
        {
            return repository;
        }

        IContextHandler IEntityEventArgs<TEntity>.GetContextHandler()
        {
            return repository;
        }

        /// <summary>
        /// The Db Entity Entry
        /// </summary>
        public DbEntityEntry EntityEntry
        {
            get { return repository.GetContext().Value.Entry(Entity); }
        }

    }
}
