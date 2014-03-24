using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security;

namespace Framework
{
    /*
     * This partial file only has overloads for Create Delete Insert and Update methods.
     * This should and does not have any logic, except for converting the parameters and calling
     * the underlining method the proper way.
     */

    public abstract partial class EntityRepository<TDbContext, TPrimaryEntity>
    {
        #region Create Overloads

        object IRepository.Create()
        {
            return Create();
        }

        #endregion Create Overloads

        #region Delete Overloads

        /// <summary>
        /// Marks an Entity for Deletion.  You must save for this entity to be deleted.
        /// </summary>
        /// <param name="entity">Entity to Delete</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Delete(TPrimaryEntity entity)
        {
            Delete(entity, false);
        }

        /// <summary>
        /// Finds the entity to delete based on its key and marks it for deletion.
        /// When the changes are saved by calling the Save method, the entity is
        /// deleted from the database.
        /// </summary>
        /// <remarks>
        ///  If the entity is not found, it is not removed and there is no warning
        ///  that it was not found.
        ///
        /// Still calls delete Core
        /// </remarks>
        /// <param name="keyValues">The key values.</param>
        public void Delete(params object[] keyValues)
        {
            TPrimaryEntity entityToDelete;
            if (keyValues.Length == 1 && keyValues[0] is TPrimaryEntity)
            {
                //Common problem when type is not casted properly
                //The developer is intending to use the regular delete method
                entityToDelete = (TPrimaryEntity)keyValues[0];
            }
            else
            {
                entityToDelete = Find(keyValues);
            } 
            if (entityToDelete != null)
            {
                DeleteCore(entityToDelete);
            }
        }

        /// <summary>
        /// Deletes a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Delete</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Delete(IEnumerable<TPrimaryEntity> entities)
        {
            Delete(entities, false);
        }

        /// <summary>
        /// Deletes a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Delete</param>
        /// <param name="save">True to save immediately after list, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Delete(IEnumerable<TPrimaryEntity> entities, bool save)
        {
            var entityArary = entities.ToArray();
            if (IsCreateAudit)
            {
                foreach (var entity in entityArary)
                {
                    //Caches original entity up front. If not it causes an error in
                    //The transaction
                    OriginalEntity(entity);
                }
            }
            WrapActionInTransaction(entityArary, (entity) => Delete(entity, false));
            Save(save);
        }

        void IRepository.Delete(object entity)
        {
            if (entity != null && entity is TPrimaryEntity)
            {
                Delete((TPrimaryEntity)entity);
            }
        }

        void IInternalRepository.Deleted(object entity)
        {
            if (entity == null || !(entity is TPrimaryEntity))
            {
                throw new InvalidCastException(string.Format("Entity must be of type {0}", TPrimaryEntityName));
            }
            Deleted((TPrimaryEntity)entity);
        }


        void IRepository<TPrimaryEntity>.Save()
        {
            this.Save();
        }

        #endregion Delete Overloads

        #region Insert Overloads

        /// <summary>
        /// Marks an Entity for Insertion.  You must save for this entity to be inserted.
        /// </summary>
        /// <param name="entity">Entity to Insert</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Insert(TPrimaryEntity entity)
        {
            Insert(entity, false);
        }

        /// <summary>
        /// Inserts a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Insert</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Insert(IEnumerable<TPrimaryEntity> entities)
        {
            Insert(entities, false);
        }

        /// <summary>
        /// Inserts a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Insert</param>
        /// <param name="save">True to save immediately after list, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Insert(IEnumerable<TPrimaryEntity> entities, bool save)
        {
            WrapActionInTransaction(entities, (entity) => Insert(entity, false));
            Save(save);
        }

        void IRepository.Insert(object entity)
        {
            if (entity != null && entity is TPrimaryEntity)
            {
                Insert((TPrimaryEntity)entity);
            }
        }

        void IInternalRepository.Inserted(object entity)
        {
            if (entity == null || !(entity is TPrimaryEntity))
            {
                throw new InvalidCastException(string.Format("Entity must be of type {0}", TPrimaryEntityName));
            }
            Inserted((TPrimaryEntity) entity);
        }

        #endregion Insert Overloads

        #region Update Overloads

        void IRepository.Update(object entity)
        {
            if (entity != null && entity is TPrimaryEntity)
            {
                Update((TPrimaryEntity)entity);
            }
        }

        /// <summary>
        /// Updates a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Update</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Update(IEnumerable<TPrimaryEntity> entities)
        {
            Update(entities, false);
        }

        /// <summary>
        /// Updates a list of Entities
        /// </summary>
        /// <param name="entities">Entities to Update</param>
        /// <param name="save">True to save immediately after list, false to delay saving</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Update(IEnumerable<TPrimaryEntity> entities, bool save)
        {
            WrapActionInTransaction(entities, (entity) => Update(entity, false));
            Save(save);
        }

        /// <summary>
        /// Marks an Entity to be Updated.  You must save for this entity to be updated.
        /// <para>Note: Keys cannot be changed when updating</para>
        /// </summary>
        /// <param name="entity">Entity to Update</param>
        /// <exception cref="DbEntityValidationException">Entity is not currently valid</exception>
        /// <exception cref="SecurityException">Current user does not have permission to do action.</exception>
        public void Update(TPrimaryEntity entity)
        {
            Update(entity, false);
        }

        void IInternalRepository.Updated(object entity)
        {
            if (entity == null || !(entity is TPrimaryEntity))
            {
                throw new InvalidCastException(string.Format("Entity must be of type {0}", TPrimaryEntityName));
            }
            Updated((TPrimaryEntity)entity);
        }


        #endregion Update Overloads

        #region Find Overloads

        object IRepository.Find(params object[] keyValues)
        {
            return Find(keyValues);
        }
        #endregion
    }
}