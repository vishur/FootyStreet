using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Framework.Entities;
using Framework.Data.Properties;
using System.Collections.ObjectModel;

namespace Framework
{
    internal static class DbContextExtensions
    {
        /// <summary>
        ///     TODO: Dependency Injection
        /// </summary>
        private static ITrackingInformation TrackingInformation
        {
            get { return new TrackingInformation(); }
        }

        public static void Save<TDbContext>(this ContextHandler<TDbContext> handler) where TDbContext : DbContext
        {
            TDbContext context = handler.GetContext().Value;

            DbChangeTracker changeTracker = context.ChangeTracker;

            changeTracker.DetectChanges();

            ValidateDateRanges(handler, changeTracker, context);

            FillTrackingInfo<TDbContext>(changeTracker);

            //changeTracker.DetectChanges(); //TODO:Potential issue becuase of the need to re-detect the changes and re-evaluate again.

            var repositories = ValidateAndApproveChanges<TDbContext>(changeTracker, handler);

            context.SaveChanges();

            foreach (IInternalRepository repository in repositories)
            {
                repository.Saved();
            }
        }

        private static List<IInternalRepository> ValidateAndApproveChanges<TDbContext>(DbChangeTracker changeTracker, IContextHandler handler) where TDbContext : DbContext
        {
            IEnumerable<Tuple<Type, IEnumerable<DbEntityEntry>>> changedEntrySet =
                GroupChangeSet<TDbContext>(changeTracker);

            var validations = new List<IInternalRepository>();
            foreach (var set in changedEntrySet)
            {
                var repository = RepositoryFactory.GetRepository(set.Item1, handler)
                                 as IInternalRepository;

                if (repository == null)
                {
                    ValidateUsingEntityFramework<TDbContext>(set);
                    continue;
                }
                validations.Add(repository);
                repository.Saving();
                if (repository.IsDefaultValidation)
                {
                    ValidateUsingEntityFramework<TDbContext>(set);
                }
                else
                {
                    ValidateUsingRepositories<TDbContext>(set, repository);
                }
                RepositoryPostActions(set, repository);

            }
            return validations;
        }

        private static void RepositoryPostActions(Tuple<Type, IEnumerable<DbEntityEntry>> set, IInternalRepository repository)
        {
            foreach (DbEntityEntry entry in set.Item2)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        repository.Inserted(entry.Entity);
                        break;

                    case EntityState.Deleted:
                        repository.Deleted(entry.Entity);
                        break;

                    case EntityState.Modified:
                        repository.Updated(entry.Entity);
                        break;
                }
            }
        }

        private static void ValidateUsingEntityFramework<TDbContext>(Tuple<Type, IEnumerable<DbEntityEntry>> set)
            where TDbContext : DbContext
        {
            //This block will use entity frame to validate
            //when it cannot find a repository.
            //This can occur for tables like Audit that are
            //Excluded from mappings
            foreach (DbEntityEntry entry in set.Item2)
            {
                DbEntityValidationResult validation = entry.GetValidationResult();
                if (!validation.IsValid)
                {
                    throw new DbEntityValidationException(String.Format(CultureInfo.InvariantCulture,
                                                                        "{0} was not valid. {1}",
                                                                        validation.GetType().Name,
                                                                        String.Join(" ",
                                                                                    validation.ValidationErrors.Select(
                                                                                        x => x.ErrorMessage))),
                                                          new List<DbEntityValidationResult> { validation });
                }
            }
        }

        private static void ValidateUsingRepositories<TDbContext>(Tuple<Type, IEnumerable<DbEntityEntry>> set,
                                                                  IInternalRepository repository)
            where TDbContext : DbContext
        {
            foreach (DbEntityEntry entry in set.Item2)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        repository.ValidateAndApprove(DataAction.Insert, entry.Entity);
                        break;

                    case EntityState.Deleted:
                        repository.ValidateAndApprove(DataAction.Delete, entry.Entity);
                        break;

                    case EntityState.Modified:
                        repository.ValidateAndApprove(DataAction.Update, entry.Entity);
                        break;
                }
            }
        }

        private static IEnumerable<Tuple<Type, IEnumerable<DbEntityEntry>>> GroupChangeSet<TDbContext>(
            DbChangeTracker changeTracker) where TDbContext : DbContext
        {
            //Here we group the changes by their type.
            //This allows us to get their repositories only
            //once from the Entity Repository factory
            IEnumerable<Tuple<Type, IEnumerable<DbEntityEntry>>> changedEntrySet =
                (from entry in changeTracker.Entries()
                 group entry by entry.Entity.GetType()
                     into grouped
                     select new Tuple<Type, IEnumerable<DbEntityEntry>>
                         (grouped.Key, grouped));
            return changedEntrySet;
        }

        private static void FillTrackingInfo<TDbContext>(DbChangeTracker changeTracker) where TDbContext : DbContext
        {
            //Tracking infomation isn't guarenteed to be filled
            //at this point, since inserts and updates can happen
            //from relationships.
            IEnumerable<IInsertTracker> insertTrackers =
                changeTracker.Entries<IInsertTracker>().
                              Where(x => x.State == EntityState.Added).
                              Select(x => x.Entity);
            MakeSureTrackingIsFilled(insertTrackers, TrackingInformation);

            IEnumerable<IUpdateTracker> updateTrackers =
                changeTracker.Entries<IUpdateTracker>().
                              Where(x => x.State == EntityState.Modified || x.State == EntityState.Added).
                              Select(x => x.Entity);
            MakeSureTrackingIsFilled(updateTrackers, TrackingInformation);
        }

        private static void ValidateDateRanges<TDbContext>(ContextHandler<TDbContext> handler,
                                                           DbChangeTracker changeTracker,
                                                           TDbContext context) where TDbContext : DbContext
        {
            //We postpone validating date ranges until save
            //So we can make sure the entire unit of work is done.
            //Instead of just validing each change
            IEnumerable<IDateRange> ranges = changeTracker.Entries<IDateRange>().Select(x => x.Entity);
            IDateRange[] dateRanges = ranges as IDateRange[] ?? ranges.ToArray();
            if (dateRanges.Any())
            {
                RemoveTimeFromRangeValues(dateRanges);

                DetectInvalidSingleRanges(dateRanges);

                FindOrCreateKeyEntities(dateRanges, handler);

                DeleteKeyEntitiesWithoutRanges(handler, changeTracker, context);

                var entityRanges = GroupRangesByKey(dateRanges, context);

                #region Validating each set of ranges.

                foreach (var set in entityRanges.Values)
                {
                    List<IDateRange> ordered = set.Item1.OrderBy(x => x.EffectiveBeginDate).ToList();
                    RangeContinuity continuity = set.Item2;
                    int count = ordered.Count;
                    for (int index = 0; index < count; index++)
                    {
                        IDateRange currentRange = ordered[index];
                        if (index == count - 1)
                        {
                            if (continuity == RangeContinuity.Continuous &&
                                currentRange.EffectiveEndDate.HasValue)
                            {
                                //Last end date needs to be null if continuous
                                currentRange.EffectiveEndDate = null;
                                DbEntityEntry<IDateRange> entry = context.Entry(currentRange);
                                if (entry.State == EntityState.Unchanged)
                                {
                                    entry.State = EntityState.Modified;
                                }
                            }
                            break;
                        }
                        DateTime nextBeginDate = ordered[index + 1].EffectiveBeginDate.Date;
                        if (!currentRange.EffectiveEndDate.HasValue)
                        {
                            if (continuity != RangeContinuity.Sequential || !currentRange.EffectiveEndDate.HasValue)
                            {
                                currentRange.EffectiveEndDate = (nextBeginDate.AddTicks(-TimeSpan.TicksPerDay)).Date;
                            }

                            DbEntityEntry<IDateRange> entry = context.Entry(currentRange);
                            if (entry.State == EntityState.Unchanged)
                            {
                                entry.State = EntityState.Modified;
                            }
                            continue;
                        }
                        DateTime currentEndDate = currentRange.EffectiveEndDate.Value.Date;
                        if (nextBeginDate <= currentEndDate)
                        {
                            var rangeType = ReflectionHelper.GetEntityType(currentRange);
                            throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                                                                        Resources.RangeCannotOverlap,
                                                                        rangeType.Name));
                        }
                        if (continuity == RangeContinuity.Continuous &&
                            currentEndDate.AddTicks(TimeSpan.TicksPerDay) != nextBeginDate)
                        {
                            var rangeType = ReflectionHelper.GetEntityType(currentRange);
                            throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                                                                        Resources.RangeMustBeAContinuous,
                                                                        rangeType.Name));
                        }
                    }
                }

                #endregion Validating each set of ranges.

                UpdateKeyEntities(handler, entityRanges);
            }


        }

        private static void DeleteKeyEntitiesWithoutRanges<TDbContext>(ContextHandler<TDbContext> handler,
            DbChangeTracker changeTracker, TDbContext context)
            where TDbContext : DbContext
        {
            var ranges = changeTracker.Entries<IDateRange>().Where(x => x.State == EntityState.Deleted).Select(x => x.Entity);
            foreach (IDateRange range in ranges)
            {
                IKeyEntity keyEntity = range.KeyEntity;

                keyEntity.RemoveChild(range);

                if (!keyEntity.Children.Any())
                {
                    context.Entry(keyEntity).State = EntityState.Deleted;
                }
            }
        }

        /// <summary>
        ///     This method will fill any Insert Tracking that doesn't have values.
        ///     This is done because an item can be inserted by association.
        /// </summary>
        /// <param name="inserts"></param>
        /// <param name="trackingInformation"></param>
        internal static void MakeSureTrackingIsFilled(IEnumerable<IInsertTracker> inserts,
                                                      ITrackingInformation trackingInformation)
        {
            var insertTrackers = inserts as IInsertTracker[] ?? inserts.ToArray();
            if (insertTrackers.Any())
            {
                string username = trackingInformation.CurrentUserId;
                DateTime time = trackingInformation.GetCurrentTime();
                foreach (IInsertTracker insert in insertTrackers)
                {
                    if (insert.CreatedBy == null)
                    {
                        insert.CreatedBy = username;
                        insert.CreatedDate = time;
                    }
                }
            }
        }

        /// <summary>
        ///     This method will fill any Update Tracking that doesn't have values.
        ///     This is done because an item can be updated by association.
        /// </summary>
        /// <param name="updates"></param>
        /// <param name="trackingInformation"></param>
        internal static void MakeSureTrackingIsFilled(IEnumerable<IUpdateTracker> updates,
                                                      ITrackingInformation trackingInformation)
        {
            IUpdateTracker[] updateTrackers
                = updates as IUpdateTracker[] ?? updates.ToArray();
            if (updateTrackers.Any())
            {
                string username = trackingInformation.CurrentUserId;
                DateTime time = trackingInformation.GetCurrentTime();
                foreach (IUpdateTracker update in updateTrackers)
                {
                    if (update.UpdatedBy == null)
                    {
                        update.UpdatedBy = username;
                        update.UpdatedDate = time;
                    }
                }
            }
        }


        private static void FindOrCreateKeyEntities(IDateRange[] dateRanges, IContextHandler handler)
        {
            foreach (IDateRange range in dateRanges)
            {
                range.FindOrCreateKeyEntity(handler);
            }
        }

        private static void RemoveTimeFromRangeValues(IEnumerable<IDateRange> allRanges)
        {
            foreach (IDateRange range in allRanges)
            {
                range.RemoveTimeFromValues();
            }
        }

        private static Dictionary<IKeyEntity, Tuple<IEnumerable<IDateRange>, RangeContinuity>> GroupRangesByKey
            (IDateRange[] dateRanges, DbContext context)
        {
            var entityRanges = new Dictionary<IKeyEntity, Tuple<IEnumerable<IDateRange>, RangeContinuity>>();
            /* We can only get an exhaustive list of all IDateRanges.
                 * but we only need each set of siblings, so grouping them
                 * by their key elimates validating the same set twice.
                 */
            foreach (IDateRange rangeItem in dateRanges)
            {
                DbEntityEntry<IDateRange> entry = context.Entry(rangeItem);
                if (entry.State == EntityState.Deleted)
                {
                    continue;
                }

                IKeyEntity key = rangeItem.KeyEntity;
                if (!entityRanges.ContainsKey(key))
                {
                    entityRanges.Add(key,
                                     new Tuple<IEnumerable<IDateRange>, RangeContinuity>(
                                         rangeItem.Siblings, rangeItem.Continuity));
                }
            }
            return entityRanges;
        }



        private static void DetectInvalidSingleRanges(IDateRange[] dateRanges)
        {
            IDateRange illegalDateRange = dateRanges.FirstOrDefault(range => range.EffectiveEndDate != null &&
                                                                             range.EffectiveBeginDate.Date >
                                                                             range.EffectiveEndDate.Value.Date);
            if (illegalDateRange != null)
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                                                            "A {0}'s date range must have the end date greater than the begin date.",
                                                            illegalDateRange.GetType().Name));
            }
        }

        internal static void ForceAttach<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            var state = context.Entry<TEntity>(entity).State;
            if (state == EntityState.Detached)
            {
                try
                {
                    context.Set<TEntity>().Attach(entity);
                }
                catch (InvalidOperationException)
                {
                    object entityRef = entity;
                    AttachEachPropertyIndividually(context, ref entityRef, typeof(TEntity), true);
                    if (!ReferenceEquals(entityRef, entity))
                    {
                        throw;
                    }
                }
            }
        }


        internal static IEnumerable<KeyValuePair<string, object>> EntityKeys(object entity)
        {
            foreach (var pair in ReflectionHelper.GetKeyProperties(entity))
            {
                yield return new KeyValuePair<string, object>(pair.Key, pair.Value.GetValue(entity));
            }
        }


        private static void AttachEachPropertyIndividually(DbContext context, ref object entity, Type entityType, bool attachRelationship)
        {
            var entry = context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                return;
            }

            var entitySetName = String.Format("{0}.{1}", context.GetType().Name.Split('.').Last(), entityType.Name.Split('.').Last());
            var e = new EntityKey(entitySetName, EntityKeys(entity));
            try
            {
                var x = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(e);
                if (!ReferenceEquals(x.Entity, entity))
                {
                    entity = x.Entity;
                    return;
                }
            }
            catch (InvalidOperationException)
            {
                
                //If not attached
            }


            var relationships = new List<Tuple<string, object, Type, DbMemberEntry>>();
            var propertyInfos = (from propertyInfo in entityType.GetProperties()
                                 let propertyType = propertyInfo.PropertyType
                                 where propertyInfo.CanRead && propertyInfo.CanWrite
                                 where propertyType.Assembly == entityType.Assembly ||
                                       (propertyType.IsGenericType &&
                                        propertyType.GetGenericArguments()[0].Assembly == entityType.Assembly)
                                 select propertyInfo).ToArray();


            foreach (var propertyInfo in propertyInfos)
            {
                var name = propertyInfo.Name;
                DbMemberEntry relationship;
                if (propertyInfo.PropertyType.IsGenericType)
                {
                    relationship = entry.Collection(name);
                }
                else
                {
                    relationship = entry.Reference(name);
                }
                try
                {
                    if (relationship.CurrentValue == null)
                    {
                        continue;
                    }
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
                relationships.Add(new Tuple<string, object, Type, DbMemberEntry>(name, relationship.CurrentValue, propertyInfo.PropertyType, relationship));
                relationship.CurrentValue = null;
            }

            context.Set(entityType).Attach(entity);

            foreach (var relationship in relationships)
            {
                if (attachRelationship)
                {
                    var collection = relationship.Item2 as IList;
                    if (collection != null)
                    {
                        for (int index = 0; index < collection.Count; index++)
                        {
                            object item = collection[index];
                            object itemRef = item;
                            AttachEachPropertyIndividually(context, ref itemRef, relationship.Item3, false);
                            collection[index] = itemRef;
                        }
                        relationship.Item4.CurrentValue = relationship.Item2;
                    }
                    else
                    {
                        object itemRef2 = relationship.Item2;
                        AttachEachPropertyIndividually(context, ref itemRef2, relationship.Item3, false);
                        relationship.Item4.CurrentValue = itemRef2;
                    }
                }
                
            }
        }


        private static void UpdateKeyEntities(IContextHandler handler, Dictionary<IKeyEntity, Tuple<IEnumerable<IDateRange>, RangeContinuity>> entityRanges)
        {
            foreach (IKeyEntity keyEntity in entityRanges.Keys)
            {
                IDateRange[] range = entityRanges[keyEntity].Item1.ToArray();
                DateTime eebd = range.First().EffectiveBeginDate.Date;
                DateTime lebd = range.Last().EffectiveBeginDate.Date;
                DateTime? leed = range.Last().EffectiveEndDate.HasValue
                                     ? range.Last().EffectiveEndDate.Value.Date
                                     : (DateTime?)null;
                if (keyEntity.EarliestEffectiveDate != eebd ||
                    keyEntity.LatestEffectiveBeginDate != lebd ||
                    keyEntity.LatestEffectiveDate != leed)
                {
                    keyEntity.EarliestEffectiveDate = eebd;
                    keyEntity.LatestEffectiveBeginDate = lebd;
                    keyEntity.LatestEffectiveDate = leed;
                    IRepository repo = RepositoryFactory.GetRepository(keyEntity.GetType(), handler);
                    repo.Update(keyEntity);
                }
                if (!range.Last().EffectiveEndDate.HasValue)
                {
                    range.Last().EffectiveEndDate = null;
                }
            }
        }
    }
}