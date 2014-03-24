using System.Data.Entity;

namespace Framework
{
    /// <summary>
    /// Represents a default instance of a Repository
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    internal sealed class DefaultRepository<TDbContext, TEntity> : EntityRepository<TDbContext, TEntity>
        where TEntity : class, new()
        where TDbContext : DbContext, new()
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DefaultRepository()
        {
            isDefaultValidation = true;
        }

        /// <summary>
        /// Checks Permissions
        /// </summary>
        /// <param name="action"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override bool CheckPermissionCore(DataAction action, TEntity entity)
        {
            //TODO: Add something simple here. Like user is logged in.
            return true;
        }
    }
}