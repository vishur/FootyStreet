using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Entities
{

    public static class DateRangeExtensions
    {

        public static void InsertOrUpdate<TEntity>(this IRepository<TEntity> repository, ref TEntity entity)
            where TEntity : class, IDateRange
        {
            var contextHandler = repository as IContextHandler;
            if (contextHandler == null)
            {
                throw new InvalidOperationException("Could not cast IRepository to IContextHandler");
            }
            entity.RemoveTimeFromValues();

            var entityBeginDate = entity.EffectiveBeginDate;

            IKeyEntity keyEntity = entity.GetKeyEntity(contextHandler);
            var exactMatch = (TEntity)keyEntity.Children.SingleOrDefault(x => x.EffectiveBeginDate == entityBeginDate);

            if (exactMatch != null)
            {
                SyncDbEntityWithDetachedEntity(ref entity, exactMatch, contextHandler);
                repository.Update(exactMatch);
            }
            else
            {
                repository.Insert(entity);
            }

            entity.FindOrCreateKeyEntity(contextHandler);

            IDateRange[] siblings;
            if (!GetSiblings(entity, out siblings))
            {
                return;
            }

            UpdateRangeBeforeEntity(repository, entity, siblings, entityBeginDate);

            UpdateRangesAfterEntity(repository, entity, siblings);
        }

        private static void SyncDbEntityWithDetachedEntity<TEntity>(ref TEntity entity, TEntity exactMatch,
                                                                    IContextHandler contextHandler)
            where TEntity : class, IDateRange
        {
            if (entity == exactMatch)
            {
                return;
            }
            var sequencePropertyName = String.Format("{0}SequenceNumber", typeof(TEntity).Name);
            var sequenceProperty = typeof(TEntity).GetProperty(sequencePropertyName);
            if (sequenceProperty != null)
            {
                var sequenceNumber = sequenceProperty.GetValue(exactMatch);
                sequenceProperty.SetValue(entity, sequenceNumber);
            }
            var entry = contextHandler.Context.Entry(exactMatch);
            entry.CurrentValues.SetValues(entity);
            entity = exactMatch;
        }

        private static void UpdateRangesAfterEntity<TEntity>(IRepository<TEntity> repository, TEntity entity, IDateRange[] siblings)
            where TEntity : class, IDateRange
        {
            var entityAfter = EntityAfter(entity, siblings);
            if (entityAfter != null)
            {
                //Insert Before an Existing item
                if (entity.EffectiveEndDate.HasValue)
                {
                    MoveRangesDown(repository, entity, entityAfter);
                }
                else
                {
                    foreach (var after in entity.Siblings.Where(x => x.EffectiveBeginDate > entity.EffectiveBeginDate))
                    {
                        repository.Delete(after);
                        entity.KeyEntity.RemoveChild(after);
                    }
                }
            }
        }

        private static void UpdateRangeBeforeEntity<TEntity>(IRepository<TEntity> repository, TEntity entity, IDateRange[] siblings,
                                                             DateTime entityBeginDate) where TEntity : class, IDateRange
        {
            var entityBefore = siblings.LastOrDefault(x => x.EffectiveBeginDate < entityBeginDate);
            if (entityBefore != null)
            {
                //Insert After an Existing item
                entityBefore.EffectiveEndDate = entity.EffectiveBeginDate - TimeSpan.FromDays(1);
                repository.Update((TEntity)entityBefore);
            }
        }


        private static bool GetSiblings<TEntity>(TEntity entity, out IDateRange[] siblings) where TEntity : class, IDateRange
        {
            var s = entity.Siblings;
            if (s.Count() == 1)
            {
                siblings = s.ToArray();
                return false;
            }
            siblings = s.OrderBy(x => x.EffectiveBeginDate).ToArray();
            return true;
        }

        private static TEntity EntityAfter<TEntity>(TEntity entity, IEnumerable<IDateRange> siblings) where TEntity : class, IDateRange
        {
            TEntity entityAfter = (TEntity)siblings.FirstOrDefault(x => x.EffectiveBeginDate > entity.EffectiveBeginDate);
            return entityAfter;
        }

        private static void MoveRangesDown<TEntity>(IRepository<TEntity> repository, TEntity entity, TEntity entityAfter)
            where TEntity : class, IDateRange
        {
            IEnumerable<IDateRange> siblings;
            var endDate = entity.EffectiveEndDate.Value;
            while (entityAfter != null &&
                   entityAfter.EffectiveEndDate.HasValue &&
                   entityAfter.EffectiveEndDate.Value < endDate)
            {
                repository.Delete(entityAfter);
                entity.KeyEntity.RemoveChild(entityAfter);
                siblings = entity.Siblings.OrderBy(x => x.EffectiveBeginDate);
                entityAfter = EntityAfter(entity, siblings);
            }

            if (entityAfter != null && entityAfter.EffectiveBeginDate != endDate + TimeSpan.FromDays(1)
                && entityAfter.EffectiveEndDate.HasValue && entityAfter.EffectiveEndDate > endDate)
            {
                var replacement = repository.Create();
                ReflectionHelper.Clone(entityAfter, replacement);
                replacement.EffectiveBeginDate = endDate + TimeSpan.FromDays(1);
                repository.Delete(entityAfter);
                repository.Insert(replacement);
            }
            else if (entityAfter != null
                     && entityAfter.EffectiveBeginDate != endDate + TimeSpan.FromDays(1)
                     && entityAfter.EffectiveEndDate == null
                     && entity.Continuity == RangeContinuity.Continuous)
            {
                entityAfter.EffectiveBeginDate = endDate + TimeSpan.FromDays(1);
            }
        }

        public static void RemoveTimeFromValues(this IDateRange range)
        {
            range.EffectiveBeginDate = range.EffectiveBeginDate.Date;
            range.EffectiveEndDate = range.EffectiveEndDate.HasValue
                                         ? range.EffectiveEndDate.Value.Date
                                         : (DateTime?)null;
        }


        internal static IKeyEntity GetKeyEntity(this IDateRange dateRange, IContextHandler handler)
        {
            if (dateRange.KeyEntity != null)
            {
                return dateRange.KeyEntity;
            }
            Type keyEntityType = dateRange.GetKeyEntityType();
            object[] fks = dateRange.GetForeignKeys();
            if (!fks.Any())
            {
                return null;
            }
            IRepository keyRepo = RepositoryFactory.GetRepository(keyEntityType, handler);
            IKeyEntity keyEntity = keyRepo.Find(fks) as IKeyEntity;
            if (keyEntity == null)
            {
                keyEntity = keyRepo.Create() as IKeyEntity;
            }
            return keyEntity;
        }

        internal static ICollection<IDateRange> LoadSiblings<TEntity>(TEntity entity, IContextHandler handler) where TEntity : class
        {
            var dateRange = entity as IDateRange;
            if (dateRange == null)
            {
                return null;
            }

            var keyEntity = dateRange.GetKeyEntity(handler);
            return keyEntity.Children.ToList();

            ////var keyType = dateRange.GetKeyEntityType();
            ////var liveProperty = keyType.GetPropertyInfo(typeof(LiveEntitiesAttribute));
            //var dateRange = entity as IDateRange;
            //if (dateRange == null)
            //{
            //    return null;
            //}
            //var fkProperties = ReflectionHelper.GetForeignKeyProperties(dateRange);  

            //var context = handler.Context;
            //var keyParameter = Expression.Parameter(typeof(TEntity));
            //Expression andExpression = null;
            //var set = context.Set<TEntity>().AsQueryable();
            //foreach (var fkProperty in fkProperties)
            //{
            //    var keyProperty = Expression.Property(keyParameter, fkProperty.Name);
            //    var rangeValue = fkProperty.GetValue(dateRange);
            //    var equalityExpression = Expression.Equal(keyProperty, Expression.Constant(rangeValue));
            //    andExpression = andExpression == null
            //                        ? equalityExpression
            //                        : Expression.And(andExpression, equalityExpression);
            //}
            //var whereLambda = Expression.Lambda<Func<TEntity, bool>>(andExpression, false, keyParameter);
            //var whereQuery = set.Where(whereLambda);

            ////var keyParameter2 = Expression.Parameter(keyType);
            ////var selectProperty = Expression.Property(keyParameter2, liveProperty);
            ////var selectLambda = Expression.Lambda(selectProperty, false, keyParameter2);
            ////var selectQuery = (ObjectQuery)whereQuery.Provider.CreateQuery(selectLambda);
            //var dbQuery = (DbQuery<TEntity>) whereQuery;
            //var query = dbQuery.ToString();
            //var connection = context.Database.Connection as SqlConnection;
            //var transaction = handler.Transaction as SqlTransaction;
            //using (var sqlCommand = new SqlCommand(query, connection, transaction))
            //{
            //    if (connection.State != ConnectionState.Open)
            //    {
            //        connection.Open();
            //    }
            //    var reader = sqlCommand.ExecuteReader();

            //    reader.Read();
            //}
            //return null;
        }

        internal static void FindOrCreateKeyEntity(this IDateRange dateRange, IContextHandler handler)
        {
            if (dateRange.KeyEntity == null)
            {
                Type keyEntityType = dateRange.GetKeyEntityType();
                object[] fks = dateRange.GetForeignKeys();
                if (!fks.Any())
                {
                    return;
                }
                IRepository keyRepo = RepositoryFactory.GetRepository(keyEntityType, handler);
                IKeyEntity keyEntity = keyRepo.Find(fks) as IKeyEntity;
                if (keyEntity != null)
                {
                    dateRange.SetKeyEntity(keyEntity);
                }
                else
                {
                    keyEntity = keyRepo.Create() as IKeyEntity;
                    dateRange.SetKeyEntityValues(keyEntity);
                    dateRange.SetKeyEntity(keyEntity);
                    keyRepo.Insert(keyEntity);
                }
                keyEntity.AddChild(dateRange);
            }
        }


        internal static Type GetKeyEntityType(this IDateRange dateRange)
        {
            var property = dateRange.GetType().GetPropertyInfo(typeof(KeyEntityAttribute));
            if (property != null)
            {
                return property.PropertyType;
            }
            return null;
        }

        internal static Type GetKeyEntityType(Type dateRangeType)
        {
            var property = dateRangeType.GetPropertyInfo(typeof(KeyEntityAttribute));
            if (property != null)
            {
                return property.PropertyType;
            }
            return null;
        }

        internal static string GetKeyEntityName(this IDateRange dateRange)
        {
            var property = dateRange.GetType().GetPropertyInfo(typeof(KeyEntityAttribute));
            if (property != null)
            {
                return property.Name;
            }
            return null;
        }

        internal static string GetKeyEntityName(Type dateRangeType)
        {
            var property = dateRangeType.GetPropertyInfo(typeof(KeyEntityAttribute));
            if (property != null)
            {
                return property.Name;
            }
            return null;
        }

        internal static string GetChildrenName(Type keyType)
        {
            var property = keyType.GetPropertyInfo(typeof(LiveEntitiesAttribute));
            if (property != null)
            {
                return property.Name;
            }
            return null;
        }

        internal static string GetSiblingPath(Type dateRangeType)
        {
            var keyProperty = GetKeyEntityName(dateRangeType);
            var keyEntityType = GetKeyEntityType(dateRangeType);
            var sibling = GetChildrenName(keyEntityType);

            if (keyProperty != null && sibling != null)
            {
                return String.Format("{0}.{1}", keyProperty, sibling);
            }
            return null;
        }

        internal static void SetKeyEntity(this IDateRange dateRange, object keyEntity)
        {
            var property = dateRange.GetType().GetPropertyInfo(typeof(KeyEntityAttribute));
            if (property != null)
            {
                property.SetValue(dateRange, keyEntity);
            }
        }


        internal static void Add(object collection, object value)
        {
            collection.GetType().GetMethod("Add").Invoke(collection, new[] { value });
        }

        internal static bool Contains(object collection, object value)
        {
            return (bool)collection.GetType().GetMethod("Contains").Invoke(collection, new[] { value });
        }

        internal static void Remove(object collection, object value)
        {
            collection.GetType().GetMethod("Remove").Invoke(collection, new[] { value });
        }

        internal static void RemoveChild(this IKeyEntity entity, object range)
        {
            object children = entity.GetType().GetPropertyInfo(typeof(LiveEntitiesAttribute)).GetValue(entity);
            Remove(children, range);
        }

        internal static void AddChild(this IKeyEntity entity, object range)
        {
            object children = entity.GetType().GetPropertyInfo(typeof(LiveEntitiesAttribute)).GetValue(entity);
            if (!Contains(children, range))
            {
                Add(children, range);
            }
        }

        internal static object[] GetForeignKeys(this IDateRange dateRange)
        {
            return ReflectionHelper.GetForeignKeyProperties(dateRange).GetPropertyValues(dateRange);
        }

        internal static void SetKeyEntityValues(this IDateRange dateRange, object keyEntity)
        {
            var dateRangeProperties = ReflectionHelper.GetForeignKeyProperties(dateRange);
            var keyEntityProperties = ReflectionHelper.GetKeyProperties(keyEntity);

            foreach (var dateRangeProperty in dateRangeProperties)
            {
                object value = dateRangeProperty.GetValue(dateRange);
                keyEntityProperties[dateRangeProperty.Name].SetValue(keyEntity, value);
            }
        }

    }
}
