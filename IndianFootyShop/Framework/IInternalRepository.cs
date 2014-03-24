using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    /// <summary>
    /// An interface for Validating an entity
    /// </summary>
    internal interface IInternalRepository: ISave
    {
        /// <summary>
        /// Validates and checks the current user's permissions
        /// </summary>
        /// <param name="action">Action to be performed</param>
        /// <param name="entity">Entity to perform action on</param>
        void ValidateAndApprove(DataAction action, object entity);

        /// <summary>
        /// Is true when no special validation occurs
        /// </summary>
        bool IsDefaultValidation { get; }

        /// <summary>
        /// Is called when on save event of the dbcontext
        /// </summary>
        void Saving();

        /// <summary>
        /// Is called when on save event of the dbcontext
        /// </summary>
        void Saved();

        void Inserted(object entity);

        void Deleted(object entity);

        void Updated(object entity);
    }
}