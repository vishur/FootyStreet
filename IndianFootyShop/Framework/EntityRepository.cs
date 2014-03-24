using Framework;
using Framework.Entities;
using Framework.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Objects;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security;

namespace Framework
{
    /// <summary>
    /// Data Access Layer Repository.
    /// </summary>
    /// <typeparam name="TDbContext">DbContext Type</typeparam>
    /// <typeparam name="TPrimaryEntity">Primary Entity Type for DbSet in DbContext</typeparam>
    public abstract partial class EntityRepository<TDbContext, TPrimaryEntity> :
        LazyContextHandler<TDbContext>, IRepository<TPrimaryEntity>, IRepository, IInternalRepository
        where TDbContext : DbContext, new()
        where TPrimaryEntity : class, new()
    {
        #region Fields

        /// <summary>
        /// Is the validation in this Repository default
        /// </summary>
        internal bool isDefaultValidation = false;

        /// <summary>
        /// Lock object for the assigning of data
        /// </summary>
        private readonly object dataThreadLock = new object();

        /// <summary>
        /// Lock object for the assigning of primarySet
        /// </summary>
        private readonly object setThreadLock = new object();

        /// <summary>
        /// private field for public Data property
        /// </summary>
        private Queryable<TPrimaryEntity, TDbContext> data;

        /// <summary>
        /// List of paths to include
        /// </summary>
        private List<string> includePaths = new List<string>();

        /// <summary>
        /// Cache of original values
        /// </summary>
        private Dictionary<TPrimaryEntity, DbPropertyValues> originalValues =
            new Dictionary<TPrimaryEntity, DbPropertyValues>();

        /// <summary>
        /// private field for public Set property
        /// </summary>
        private DbSet<TPrimaryEntity> primarySet = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Empty constructor
        /// </summary>
        protected EntityRepository()
        {
            if (IsDateRange)
            {
                string keyName = DateRangeExtensions.GetKeyEntityName(typeof(TPrimaryEntity));
                if (keyName != null)
                {
                    Include(keyName);
                }
            }
        }

        /// <summary>
        /// Constructor which shares a DbContext with another ContextHandler.
        /// </summary>
        /// <param name="providingContext">Object that has a Context to share with this Repository</param>
        protected EntityRepository(ContextHandler<TDbContext> providingContext)
            : this()
        {
            ShareContext(providingContext);
        }

        #endregion Constructors

        #region Public and Protected Properties

        /// <summary>
        /// Default All Query For TPrimaryEntity
        /// </summary>
        /// <remarks>
        /// Overriding ApplyAccessRestriction will restrict the Data from this query.
        /// </remarks>
        public Queryable<TPrimaryEntity, TDbContext> Data
        {
            get
            {
                if (data == null)
                {
                    lock (dataThreadLock)
                    {
                        if (data == null)
                        {
                            DbQuery<TPrimaryEntity> query = Set.AsNoTracking();
                            foreach (string path in includePaths)
                            {
                                query = query.Include(path);
                            }
                            data = AddContext(
                                ApplyAccessRestrictions(query));
                        }
                    }
                }
                return data;
            }
        }

        internal static ITrackingInformation TrackingInformation
        {
            get { return new TrackingInformation(); }
        }

        /// <summary>
        /// TPrimaryEntity's Set from TDbContext
        /// </summary>
        protected DbSet<TPrimaryEntity> Set
        {
            get
            {
                if (primarySet == null)
                {
                    lock (setThreadLock)
                    {
                        if (primarySet == null)
                        {
                            primarySet = Context.Set<TPrimaryEntity>();
                        }
                    }
                }
                return primarySet;
            }
        }

        #endregion Public and Protected Properties

        #region Methods

        /// <summary>
        /// This method is designed to see if the current user executing has the proper
        /// role/access/permisions to carry out the task
        /// </summary>
        /// <param name="action">Action(s) the user is trying to perform.</param>
        /// <param name="entity">Entity the which to perform action(s) on.</param>
        /// <returns>True for having Permission, False for not</returns>
        /// <remarks>
        /// This is executed before attempting to do actions, and will throw a Security exception if it is false.
        /// </remarks>
        public bool CheckPermission(DataAction action, TPrimaryEntity entity)
        {
            return CheckPermissionCore(action, entity);
        }

        /// <summary>
        /// Creates a new instance of an entity for the type of this set. This instance is not added or
        /// attached to the set. The instance returned will be a proxy if the underlying context is
        /// configured to create proxies and the entity type meets the requirements for creating a proxy.
        /// </summary>
        /// <returns></returns>
        public TPrimaryEntity Create()
        {
            TPrimaryEntity entity = Set.Create();
            return CheckPermissionCore(DataAction.Create, entity) ? entity : null;
        }

        /// <summary>
        /// Deletes an Entity
        /// </summary>
        /// <param name="entity">Entity to Delete</param>
        /// <param name="save">True to save immediately, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Delete(TPrimaryEntity entity, bool save)
        {
            ValidateAndApprove(DataAction.Delete, entity);
            DeleteCore(entity);
            Save(save);
        }

        /// <summary>
        /// Removes a list of include statements
        /// </summary>
        /// <param name="names"></param>
        public void Exclude(params string[] paths)
        {
            lock (dataThreadLock) // I don't think this is thread safe
            {
                foreach (string path in paths)
                {
                    includePaths.Remove(path);
                }
                data = null;
            }
        }

        /// <summary>
        /// Uses the primary key value to attempt to find an entity tracked by the context.
        /// If the entity is not in the context then a query will be executed and evaluated
        /// against the data in the data source, and null is returned if the entity is not found
        /// in the context, in the data source, or user does not have permission to read the
        /// entity. Note that the Find also returns entities that have been added to the
        /// context but have not yet been saved to the database.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>TEntity</returns>
        /// <remarks>
        /// The Find method takes an array of objects as an argument. When working
        /// with composite primary keys, pass the key values separated by commas
        /// and in the same order that they are defined in the model.
        /// </remarks>
        public TPrimaryEntity Find(params object[] keyValues)
        {
            TPrimaryEntity entity = FindCore(keyValues);
            return CheckPermissionCore(DataAction.Read, entity) ? entity : (TPrimaryEntity)null;
        }

        /// <summary>
        /// Validates tracked entities and returns a Collection of <see cref="DbEntityValidationResult"/>
        /// containing validation results.
        /// </summary>
        /// <returns>
        /// Collection of validation results for invalid entities.
        /// The collection is never Nothing and must not contain Nothing values or results for valid entities.
        /// </returns>
        public IEnumerable<DbEntityValidationResult> GetValidationErrors()
        {
            return (from error in Context.GetValidationErrors()
                    where error.Entry.Entity.GetType() is TPrimaryEntity
                    where !error.IsValid
                    select error);
        }

        /// <summary>
        /// Validates a list of entities and returns a Collection of <see cref="DbEntityValidationResult"/>
        /// containing validation results.
        /// </summary>
        /// <returns>
        /// Collection of validation results for invalid entities.
        /// The collection is never Nothing and must not contain Nothing values or results for valid entities.
        /// </returns>
        public IEnumerable<DbEntityValidationResult> GetValidationErrors(IEnumerable<TPrimaryEntity> entities)
        {
            foreach (TPrimaryEntity entity in entities)
            {
                var validation = Validate(entity);
                if (!validation.IsValid)
                {
                    yield return validation;
                }
            }
        }

        /// <summary>
        /// Validates a list of entities and returns a Collection of <see cref="DbEntityValidationResult"/>
        /// containing validation results.
        /// </summary>
        /// <returns>
        /// Collection of validation results for invalid entities.
        /// The collection is never Nothing and must not contain Nothing values or results for valid entities.
        /// </returns>
        public IEnumerable<DbEntityValidationResult> GetValidationErrors(params TPrimaryEntity[] entities)
        {
            return GetValidationErrors(entities.AsEnumerable());
        }

        /// <summary>
        /// Adds a list of include statements
        /// </summary>
        /// <param name="names"></param>
        public void Include(params string[] paths)
        {
            lock (dataThreadLock) // I don't think this is thread safe
            {
                includePaths.AddRange(paths);
                includePaths = includePaths.Distinct().ToList();
                data = null;
            }
        }

        /// <summary>
        /// Inserts an Entity
        /// </summary>
        /// <param name="entity">Entity to Insert</param>
        /// <param name="save">True to save immediately, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Insert(TPrimaryEntity entity, bool save)
        {
            Inserting(entity);
            ValidateAndApprove(DataAction.Insert, entity);
            Set.Add(entity);
            Save(save);
        }


        /// <summary>
        /// Disconnected Update Method.
        /// <para>Note: Keys cannot be changed when updating</para>
        /// </summary>
        /// <param name="entity">Entity to Update</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Update(TPrimaryEntity entity, bool save)
        {
            var state = Context.Entry<TPrimaryEntity>(entity).State;
            if (state == EntityState.Added)
            {
                ValidateAndApprove(DataAction.Insert, entity);
                Save(save);
                return;
            }

            Updating(entity);
            ValidateAndApprove(DataAction.Update, entity);

            if (state == EntityState.Detached)
            {
                Set.Attach(entity);
            }

            Context.Entry<TPrimaryEntity>(entity).State = EntityState.Modified;
            Save(save);
        }

        /// <summary>
        /// Validates an entity's values.
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns></returns>
        public DbEntityValidationResult Validate(TPrimaryEntity entity)
        {
            DbEntityValidationResult results =
                Context.Entry<TPrimaryEntity>(entity).GetValidationResult();
            foreach (var additional in ValidateCore(entity))
            {
                results.ValidationErrors.Add(additional);
            }
            return results;
        }

        /// <summary>
        /// Adds the local context to a Queryable Object
        /// </summary>
        /// <param name="query">Query to add context to</param>
        /// <returns></returns>
        protected Queryable<TPrimaryEntity, TDbContext> AddContext(IQueryable<TPrimaryEntity> query)
        {
            return new Queryable<TPrimaryEntity, TDbContext>(query, this);
        }

        /// <summary>
        /// Restricts the public get Data Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual IQueryable<TPrimaryEntity> ApplyAccessRestrictions(IQueryable<TPrimaryEntity> query)
        {
            return query;
        }

        /// <summary>
        /// This method is designed to see if the current user executing has the proper
        /// role/access/permisions to carry out the task
        /// </summary>
        /// <param name="action">Action(s) the user is trying to perform.</param>
        /// <param name="entity">Entity the which to perform action(s) on.</param>
        /// <returns>True for having Permission, False for not</returns>
        /// <remarks>
        /// This is executed before attempting to do actions, and will throw a Security exception if it is false.
        /// 
        /// Warning, Do not have more persmissions on a dependent table.  IE: A if you cannot insert a key entity, you
        /// will not be able to insert a live entity.
        /// </remarks>
        protected abstract bool CheckPermissionCore(DataAction action, TPrimaryEntity entity);

        /// <summary>
        /// Default delete method
        /// </summary>
        /// <param name="entity">Entities to Delete</param>
        /// <remarks>Override this method to customize deleting</remarks>
        protected virtual void DeleteCore(TPrimaryEntity entity)
        {
            var entry = Context.Entry(entity);
            var state = entry.State;
            if (state == EntityState.Detached)
            {
                Set.Attach(entity);
            }

            if (IsCreateAudit)
            {
                CreateAudit(entity, true);
            }

            if (IsDateRange)
            {
                DateRangeExtensions.LoadSiblings(entity, this);
            }

            if (state != EntityState.Added)
            {
                Set.Remove(entity);
            }
            else
            {
                entry.State = EntityState.Detached;
            }
        }



        private void Deleted(TPrimaryEntity entity)
        {
            if (DeletedEvent != null)
            {
                DeletedEvent(this, new EntityEventArgs<TDbContext, TPrimaryEntity>(this, entity));
            }
        }

        /// <summary>
        /// Disposing local members
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                data = null;
                primarySet = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Uses the primary key value to attempt to find an entity tracked by the context.
        /// If the entity is not in the context then a query will be executed and evaluated
        /// against the data in the data source, and null is returned if the entity is not found
        /// in the context, in the data source, or user does not have permission to read the
        /// entity. Note that the Find also returns entities that have been added to the
        /// context but have not yet been saved to the database.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>TEntity</returns>
        /// <remarks>
        /// The Find method takes an array of objects as an argument. When working
        /// with composite primary keys, pass the key values separated by commas
        /// and in the same order that they are defined in the model.
        /// </remarks>
        protected virtual TPrimaryEntity FindCore(params object[] keyValues)
        {
            return Set.Find(keyValues);
        }

        /// <summary>
        /// Gets another Repository and attempts to share the context with it.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected IRepository<TEntity> GetRepository<TEntity>()
        {
            return RepositoryFactory.GetRepository<TEntity>(this);
        }

        /// <summary>
        /// Gets another Repository and attempts to share the context with it.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected object GetRepository(Type entityType)
        {
            return RepositoryFactory.GetRepository(entityType, this);
        }

        /// <summary>
        /// Default insert method
        /// </summary>
        /// <param name="entity">Entities to Insert</param>
        /// <remarks>Override this method to customize inserting</remarks>
        protected virtual void Inserted(TPrimaryEntity entity)
        {
            if (InsertedEvent != null)
            {
                InsertedEvent(this, new EntityEventArgs<TDbContext, TPrimaryEntity>(this, entity));
            }
        }

        protected virtual void Inserting(TPrimaryEntity entity)
        {
            if (IsInsertTracker)
            {
                IInsertTracker tracker = (IInsertTracker)entity;
                tracker.CreatedBy = TrackingInformation.CurrentUserId;
                tracker.CreatedDate = TrackingInformation.GetCurrentTime();
            }
            if (IsUpdateTracker)
            {
                IUpdateTracker tracker = (IUpdateTracker)entity;
                tracker.UpdatedBy = TrackingInformation.CurrentUserId;
                tracker.UpdatedDate = TrackingInformation.GetCurrentTime();
            }
        }

        /// <summary>
        /// Returns an object with the original values of the entity provided
        /// Makes a call to the database to get the original values the first time
        /// then it gets it from cache.
        /// </summary>
        /// <param name="entity">Entity to get original values from.</param>
        /// <returns></returns>
        protected TPrimaryEntity OriginalEntity(TPrimaryEntity entity)
        {
            var propertyValues = OriginalPropertyValues(entity);
            return propertyValues == null ? null : propertyValues.ToObject() as TPrimaryEntity;
        }

        /// <summary>
        /// Method is called when Save has completed.
        /// This method is called reguardless of which repository
        /// calls Save in a shared transaction.
        /// </summary>
        protected virtual void Saved()
        {
            originalValues.Clear();
        }

        /// <summary>
        /// Default update method
        /// </summary>
        /// <param name="original">Entity to Update</param>
        /// <param name="updated">New values for entity</param>
        /// <remarks>Override this method to customize inserting</remarks>
        protected virtual void Updated(TPrimaryEntity entity)
        {
            CreateAudit(entity, false);

            if (UpdatedEvent != null)
            {
                UpdatedEvent(this, new EntityEventArgs<TDbContext, TPrimaryEntity>(this, entity));
            }
        }

        /// <summary>
        /// Context has been changed
        /// </summary>
        protected override void UpdatedContext()
        {
            base.UpdatedContext();
            primarySet = null;
            data = null;
        }

        /// <summary>
        /// Default update method
        /// </summary>
        /// <param name="updated">New values for entity</param>
        /// <remarks>Override this method to customize inserting</remarks>
        protected virtual void Updating(TPrimaryEntity entity)
        {
            if (IsUpdateTracker && IsInsertTracker)
            {
                //Making sure values were not overwriten externally.
                IInsertTracker insertInfo = (IInsertTracker)OriginalEntity(entity);
                IInsertTracker updated = (IInsertTracker)entity;
                if (insertInfo != null)
                {
                    updated.CreatedDate = insertInfo.CreatedDate;
                    updated.CreatedBy = insertInfo.CreatedBy;
                }
                // Context.Entry<TPrimaryEntity>(entity).State = EntityState.Modified;
            }

            if (IsUpdateTracker)
            {
                IUpdateTracker tracker = (IUpdateTracker)entity;
                tracker.UpdatedBy = TrackingInformation.CurrentUserId;
                tracker.UpdatedDate = TrackingInformation.GetCurrentTime();
            }

                // When there is an Audit Logic, and your primary entity does not
            // implement IUpdateTracker.  We assume Insert tracker is used to also
            // track updating.
            else if (IsInsertTracker && IsCreateAudit)
            {
                IInsertTracker tracker = (IInsertTracker)entity;
                tracker.CreatedBy = TrackingInformation.CurrentUserId;
                tracker.CreatedDate = TrackingInformation.GetCurrentTime();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual IEnumerable<DbValidationError> ValidateCore(TPrimaryEntity entity)
        {
            return Enumerable.Empty<DbValidationError>();
        }

        /// <summary>
        /// Performs actions for a list of entities.
        /// If one action fails it rolls back all the changes.
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="action">Action to perform</param>
        protected void WrapActionInTransaction(IEnumerable<TPrimaryEntity> entities,
                                               Action<TPrimaryEntity> action)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities", "Argument cannot be null.");
            }
            List<TPrimaryEntity> list = entities.ToList();
            WrapActionInTransaction(() =>
            {
                foreach (TPrimaryEntity entity in list)
                {
                    action(entity);
                }
            });

        }

        #endregion Methods

        #region Internal Members

        /// <summary>
        /// The Type Name for the Primary Entity
        /// </summary>
        internal static string TPrimaryEntityName
        {
            get { return typeof(TPrimaryEntity).Name; }
        }


        /// <summary>
        /// Creates and Inserts an Audit entity based off of the Live entity
        /// <para>Does nothing when Audit is not enabled.</para>
        /// </summary>
        /// <param name="entity">Entity to Audit</param>
        internal virtual void CreateAudit(TPrimaryEntity entity, bool isDeleted)
        {
            if (IsCreateAudit)
            {
                TPrimaryEntity original = OriginalEntity(entity);
                if (original != null)
                {
                    InsertAudit(((ICreateAudit<IAuditTracker>)original).CreateAudit(), isDeleted);
                }
            }
        }

        /// <summary>
        /// Inserts an Audit Entity.
        /// </summary>
        /// <param name="auditEntity"></param>
        internal virtual void InsertAudit(IAuditTracker auditEntity, bool isDeleted)
        {
            auditEntity.AuditUserId = TrackingInformation.CurrentUserId;
            auditEntity.AuditDate = TrackingInformation.GetCurrentTime();
            auditEntity.IsDeleted = isDeleted;
            Context.Set(AuditEntityType).Add(auditEntity);
        }

        /// <summary>
        /// Returns the original property values
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal DbPropertyValues OriginalPropertyValues(TPrimaryEntity entity)
        {
            var state = Context.Entry<TPrimaryEntity>(entity).State;
            if (state == EntityState.Detached)
            {
                //Set.Attach(entity);
                Context.ForceAttach(entity);
            }
            if (state == EntityState.Added)
            {
                return null;
            }
            DbPropertyValues original;
            if (!originalValues.ContainsKey(entity))
            {
                original = Context.Entry<TPrimaryEntity>(entity).GetDatabaseValues();
                originalValues.Add(entity, original);
            }
            else
            {
                original = originalValues[entity];
            }
            return original;
        }

        /// <summary>
        /// Commits all changes to the underlining database
        /// </summary>
        /// <param name="save">
        /// True to actually save, False not to save
        /// </param>
        internal void Save(bool save)
        {
            if (save)
            {
                this.Save();
            }
        }

        /// <summary>
        /// Validates and checks the current user's permissions
        /// </summary>
        /// <param name="action">Action to be performed</param>
        /// <param name="entity">Entity to perform action on</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        internal void ValidateAndApprove(DataAction action, TPrimaryEntity entity)
        {
            if (HasDisposed)
            {
                throw new ObjectDisposedException("Entity Repository has already been disposed.");
            }

            //We don't need to valid something we are deleting
            if (action != DataAction.Delete)
            {
                var validation = Validate(entity);
                if (!validation.IsValid)
                {
                    throw new DbEntityValidationException(String.Format(CultureInfo.InvariantCulture,
                                                                        "{0} was not valid. {1}", TPrimaryEntityName,
                                                                        String.Join(" ",
                                                                                    validation.ValidationErrors.Select(
                                                                                        x => x.ErrorMessage))),
                                                          new List<DbEntityValidationResult> { validation });
                }
            }

            if (!CheckPermissionCore(action, entity))
            {
                throw new SecurityException(String.Format(CultureInfo.InvariantCulture,
                                                          "Could not perform action {0} on entity {1}.",
                                                          Enum.GetName(typeof(DataAction), action), TPrimaryEntityName));
            }
        }

        #endregion Internal Members

        #region Explicit Interface Methods

        /// <summary>
        /// Returns a Queryable for the set of data
        /// </summary>
        IQueryable<TPrimaryEntity> IRepository<TPrimaryEntity>.Data
        {
            get
            {
                return this.Data; //Non Explicit
            }
        }

        /// <summary>
        /// Determines if the Validation is just the default Validation.
        /// </summary>
        /// <returns></returns>
        bool IInternalRepository.IsDefaultValidation
        {
            get { return isDefaultValidation; }
        }

        /// <summary>
        /// Is called when on save event of the dbcontext
        /// </summary>
        void IInternalRepository.Saved()
        {
            Saved();
        }

        /// <summary>
        /// Is called when on save event of the dbcontext
        /// </summary>
        void IInternalRepository.Saving()
        {
        }

        /// <summary>
        /// Validates and checks the current user's permissions
        /// </summary>
        /// <param name="action">Action to be performed</param>
        /// <param name="entity">Entity to perform action on</param>
        void IInternalRepository.ValidateAndApprove(DataAction action, object entity)
        {
            this.ValidateAndApprove(action, entity as TPrimaryEntity);
        }

        #endregion Explicit Interface Methods

        #region Events

        internal static event EventHandler<IEntityEventArgs> DeletedEvent;

        internal static event EventHandler<IEntityEventArgs> InsertedEvent;

        internal static event EventHandler<IEntityEventArgs> UpdatedEvent;

        #endregion Events

        void ISave.Save()
        {
            this.Save();
        }


    }
}