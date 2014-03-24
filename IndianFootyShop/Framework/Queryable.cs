using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Framework
{
    /// <summary>
    /// A Queryable entity that also contains the
    /// ability to share a connection
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class Queryable<TDbContext> : ContextHandler<TDbContext>, IQueryable where TDbContext : DbContext
    {
        private IQueryable Query { get; set; }

        public Queryable(IQueryable query, ContextHandler<TDbContext> providingHandler)
        {
            Query = query;
            ShareContext(providingHandler);
        }

        #region IQueryable Members

        public Type ElementType
        {
            get { return Query.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return Query.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return Query.Provider; }
        }

        #endregion IQueryable Members

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        #endregion IEnumerable Members
    }

    /// <summary>
    /// A Queryable entity that also contains the
    /// ability to share a connection
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class Queryable<TEntity, TDbContext> : Queryable<TDbContext>,
        IQueryable<TEntity> where TDbContext : DbContext
    {
        protected IQueryable<TEntity> Query { get; set; }

        protected internal Queryable(IQueryable<TEntity> query, ContextHandler<TDbContext> handler)
            : base(query, handler)
        {
            Query = query;
        }

        #region IEnumerable<TEntity> Members

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        #endregion IEnumerable<TEntity> Members
    }
}