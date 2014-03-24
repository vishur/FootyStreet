using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security;

namespace Framework
{
    /// <summary>
    /// Repesents the interface for basic repository functionality
    /// </summary>
    /// <typeparam name="TEntity">The entity to do work on</typeparam>
    public interface IRepository<TEntity> : IDisposable //where TEntity : class
    {
        /// <summary>
        /// Default All Query For TEntity
        /// </summary>
        /// <remarks>
        /// Overriding ApplyAccessRestriction will restrict the Data from this query.
        /// </remarks>
        IQueryable<TEntity> Data { get; }

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
        bool CheckPermission(DataAction action, TEntity entity);

        /// <summary>
        /// Creates a new instance of an entity for the type of this set. This instance is not added or
        /// attached to the set. The instance returned will be a proxy if the underlying context is
        /// configured to create proxies and the entity type meets the requirements for creating a proxy.
        /// </summary>
        /// <returns></returns>
        TEntity Create();

        /// <summary>
        /// Marks an Entity for Deletion.  You must save for this entity to be deleted.
        /// </summary>
        /// <param name="entity">Entity to Delete</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes an Entity
        /// </summary>
        /// <param name="entity">Entity to Delete</param>
        /// <param name="save">True to save immediately, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Delete(TEntity entity, bool save);

        /// <summary>
        /// Finds the entity to delete based on its key and marks it for deletion.
        /// When the changes are saved by calling the Save method, the entity is
        /// deleted from the database.
        /// </summary>
        /// <remarks>
        /// If the entity is not found, it is not removed and there is no warning
        /// that it was not found.
        ///
        /// Still calls delete Core
        /// </remarks>
        /// <param name="keyValues">The key values.</param>
        void Delete(params object[] keyValues);

        /// <summary>
        /// Deletes a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Delete</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Delete</param>
        /// <param name="save">True to save immediately after list, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Delete(IEnumerable<TEntity> entities, bool save);

        /// <summary>
        /// Removes a list of include statements
        /// </summary>
        /// <param name="names"></param>
        void Exclude(params string[] paths);

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
        TEntity Find(params object[] keyValues);

        /// <summary>
        /// Validates tracked entities and returns a Collection of <see cref="DbEntityValidationResult"/>
        /// containing validation results.
        /// </summary>
        /// <returns>
        /// Collection of validation results for invalid entities.
        /// The collection is never Nothing and must not contain Nothing values or results for valid entities.
        /// </returns>
        IEnumerable<DbEntityValidationResult> GetValidationErrors();

        /// <summary>
        /// Validates a list of entities and returns a Collection of <see cref="DbEntityValidationResult"/>
        /// containing validation results.
        /// </summary>
        /// <returns>
        /// Collection of validation results for invalid entities.
        /// The collection is never Nothing and must not contain Nothing values or results for valid entities.
        /// </returns>
        IEnumerable<DbEntityValidationResult> GetValidationErrors(IEnumerable<TEntity> entities);

        /// <summary>
        /// Validates a list of entities and returns a Collection of <see cref="DbEntityValidationResult"/>
        /// containing validation results.
        /// </summary>
        /// <returns>
        /// Collection of validation results for invalid entities.
        /// The collection is never Nothing and must not contain Nothing values or results for valid entities.
        /// </returns>
        IEnumerable<DbEntityValidationResult> GetValidationErrors(params TEntity[] entities);

        /// <summary>
        /// Adds a list of include statements
        /// </summary>
        void Include(params string[] paths);

        /// <summary>
        /// Marks an Entity for Insertion.  You must save for this entity to be inserted.
        /// </summary>
        /// <param name="entity">Entity to Insert</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Insert(TEntity entity);

        /// <summary>
        /// Inserts an Entity
        /// </summary>
        /// <param name="entity">Entity to Insert</param>
        /// <param name="save">True to save immediately, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Insert(TEntity entity, bool save);

        /// <summary>
        /// Inserts a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Insert</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Inserts a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Insert</param>
        /// <param name="save">True to save immediately after list, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Insert(IEnumerable<TEntity> entities, bool save);

        /// <summary>
        /// Commits all changes to the underlining database
        /// </summary>
        void Save();

        /// <summary>
        /// Updates a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Update</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Update</param>
        /// <param name="save">True to save immediately after list, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Update(IEnumerable<TEntity> entities, bool save);

        /// <summary>
        /// Marks an Entity to be Updated.  You must save for this entity to be updated.
        /// <para>Note: Keys cannot be changed when updating</para>
        /// </summary>
        /// <param name="entity">Entity to Update</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Update(TEntity entity);

        /// <summary>
        /// Disconnected Update Method.
        /// <para>Note: Keys cannot be changed when updating</para>
        /// </summary>
        /// <param name="entity">Entity to Update</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        void Update(TEntity entity, bool save);

        /// <summary>
        /// Validates an entity's values.
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns></returns>
        DbEntityValidationResult Validate(TEntity entity);
    }

    /// <summary>
    /// Object based Repository
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Creates an object
        /// </summary>
        /// <returns></returns>
        object Create();

        /// <summary>
        /// Uses the primary key value to attempt to find an entity tracked by the context.
        /// If the entity is not in the context then a query will be executed and evaluated
        /// against the data in the data source, and null is returned if the entity is not found
        /// in the context, in the data source, or user does not have permission to read the
        /// entity. Note that the Find also returns entities that have been added to the
        /// context but have not yet been saved to the database.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>Entity</returns>
        /// <remarks>
        /// The Find method takes an array of objects as an argument. When working
        /// with composite primary keys, pass the key values separated by commas
        /// and in the same order that they are defined in the model.
        /// </remarks>
        object Find(params object[] keyValues);

        /// <summary>
        /// Deletes an object
        /// </summary>
        /// <param name="entity"></param>
        void Delete(object entity);

        /// <summary>
        /// Inserts an object
        /// </summary>
        /// <param name="entity"></param>
        void Insert(object entity);

        /// <summary>
        /// Updates an object
        /// </summary>
        /// <param name="entity"></param>
        void Update(object entity);
    }
}