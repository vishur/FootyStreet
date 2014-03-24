using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace Framework
{
    public static class DataExtensions
    {
        /// <summary> 
        /// Execute a command against the database server that does not return a sequence of objects.
        /// The command is specified using the server's native query language, such as SQL.
        /// </summary>
        /// <param name="context">The Context</param>
        /// <param name="commandName">The command specified in the server's native query language.</param> 
        /// <param name="parameters">The parameter values to use for the query.</param>
        /// <returns>A single integer return value</returns> 
        public static int ExecuteStoreCommand(this DbContext context, string commandName, params object[] parameters)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            IObjectContextAdapter adapter = context;
            ObjectContext objectContext = adapter.ObjectContext;
            return objectContext.ExecuteStoreCommand(commandName, parameters);
        }


        /// <summary>
        /// Executes a stored procedure or function that is defined in the data source and expressed in the conceptual model; discards any results returned from the function; and returns the number of rows affected by the execution.
        /// </summary>        
        /// <param name="context">The Context</param>
        /// <param name="commandName">The command specified in the server's native query language.</param> 
        /// <param name="parameters">The parameter values to use for the query.</param>
        /// <returns></returns>
        public static int ExecuteFunction(this DbContext context, string commandName, params ObjectParameter[] parameters)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            IObjectContextAdapter adapter = context;
            ObjectContext objectContext = adapter.ObjectContext;
            return objectContext.ExecuteFunction(commandName, parameters);
        }

        /// <summary> 
        /// Execute the sequence returning query against the database server.
        /// The query is specified using the server's native query language, such as SQL. 
        /// </summary> 
        /// <typeparam name="TElement">The element type of the result sequence.</typeparam>
        /// <param name="context">The Context</param>
        /// <param name="commandName">The command specified in the server's native query language.</param> 
        /// <param name="parameters">The parameter values to use for the query.</param>
        /// <returns>An IEnumerable sequence of objects.</returns>
        public static IEnumerable<TElement> ExecuteStoreQuery<TElement>(this DbContext context, string commandName, params object[] parameters)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            IObjectContextAdapter adapter = context;
            ObjectContext objectContext = adapter.ObjectContext;
            return objectContext.ExecuteStoreQuery<TElement>(commandName, parameters);
        }

        /// <summary>
        /// Deletes a set of data based off a lambda expression
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="repository">The Repository</param>
        /// <param name="predicate">Filters data based on predicate</param>
        public static void Delete<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            repository.Delete(repository.Data.Where(predicate));
        }

        public static void PerformAction<TEntity>(this IRepository<TEntity> repository, TEntity entity,
                                                  RepositoryAction action)
        {
            switch (action)
            {
                case RepositoryAction.Delete:
                    repository.Delete(entity);
                    break;
                case RepositoryAction.Insert:
                    repository.Insert(entity);
                    break;
                case RepositoryAction.Update:
                    repository.Update(entity);
                    break;
            }
        }
       
    }
}
