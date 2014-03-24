using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    /// <summary>
    /// Determines how to use the property paths in the update statement
    /// </summary>
    public enum UpdatePropertyMode
    {
        /// <summary>
        /// Will only update the columns provided
        /// </summary>
        IncludeProperties,

        /// <summary>
        /// Will not update the columns provide, but all others
        /// </summary>
        ExcludeProperties
    }

}
