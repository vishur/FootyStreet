using System;
using System.Data.Entity;
using System.Reflection;
using System.Linq;

namespace Framework.Events
{
    /// <summary>
    /// Registers an Event to a Repository
    /// </summary>
    public static class EventRegistration
    {
        private const string EventSuffix = "Event";
        
        
        /*public static void AddEventHandler<TDbContext, TEntity>(
            EventHandler<EntityEventArgs<TDbContext, TEntity>> eventHandler, EntityEvent entityEvent)
                where TDbContext : DbContext, new()
                where TEntity : class, new()
        {
            Type genericType = typeof (EntityRepository<TDbContext, TEntity>);
            var genericEvent = GetEvent(genericType, entityEvent);
            genericEvent.GetAddMethod(true).Invoke(null, new object[] {eventHandler});
        }*/

        /// <summary>
        /// Adds an Event Handler to an entity event
        /// </summary>
        /// <param name="eventHandler">The method to call</param>
        /// <param name="entityEvent">On which event</param>
        /// <param name="entityType">For which entity type</param>
        public static void AddEventHandler(EventHandler<IEntityEventArgs> eventHandler, EntityEvent entityEvent, Type entityType)
        {
            IContextHandler handler = RepositoryFactory.GetRepository(entityType) as IContextHandler;
            if (handler == null)
            {
                throw new InvalidOperationException("Could not find repository to register to.");
            }
            Type genericType = typeof (EntityRepository<,>).MakeGenericType(handler.ContextType, entityType);
            var genericEvent = GetEvent(genericType, entityEvent);
            genericEvent.GetAddMethod(true).Invoke(null, new object[] { eventHandler });
        }

        /*public static void RemoveEventHandler<TDbContext, TEntity>(
            EventHandler<EntityEventArgs<TDbContext, TEntity>> eventHandler, EntityEvent entityEvent)
            where TDbContext : DbContext, new()
            where TEntity : class, new()
        {
            Type genericType = typeof(EntityRepository<TDbContext, TEntity>);
            var genericEvent = GetEvent(genericType, entityEvent);
            genericEvent.GetRemoveMethod(true).Invoke(null, new object[] { eventHandler });
        }*/

        /// <summary>
        /// Removes an Event Handler to an entity event
        /// </summary>
        /// <param name="eventHandler">The method to call</param>
        /// <param name="entityEvent">On which event</param>
        /// <param name="entityType">For which entity type</param>
        public static void RemoveEventHandler(EventHandler<IEntityEventArgs> eventHandler, EntityEvent entityEvent, Type entityType)
        {
            IContextHandler handler = RepositoryFactory.GetRepository(entityType) as IContextHandler;
            if (handler == null)
            {
                throw new InvalidOperationException("Could not find repository to register to.");
            }
            Type genericType = typeof(EntityRepository<,>).MakeGenericType(handler.ContextType, entityType);
            var genericEvent = GetEvent(genericType, entityEvent);
            genericEvent.GetRemoveMethod(true).Invoke(null, new object[] { eventHandler });
        }

        private static EventInfo GetEvent(Type genericType, EntityEvent entityEvent)
        {
            string eventName = entityEvent.ToString() + EventSuffix;
            return genericType.GetEvent(eventName, BindingFlags.NonPublic | BindingFlags.Static);
        }
    }
}
