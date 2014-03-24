using Framework.Entities;
using System;
using System.ComponentModel;
using System.Data.Entity;

namespace Framework.Entities
{
    /// <summary>
    /// Creates an Audit Entity from the definition of the Live Entity.
    /// <para>By default the system uses the convention of the suffix 'Audit' to locate Audit Entities.</para>
    /// <para>If the Audit Entity does not end with Audit, you can use this interface to introduce</para>
    /// <para>the Audit by configuration.  Using this interface will also explicitly create the Audit</para>
    /// <para>Entity, which will be faster than dynamically creating it.</para>
    /// </summary>
    /// <typeparam name="TAudit">The type of the audit entity</typeparam>
    /// <remarks>
    /// This is a covariance generic on purpose.
    /// </remarks>
    /// <example>
    /// ICreateAudit&lt;IAuditTracker&gt; creator = liveObject as ICreateAudit&lt;IAuditTracker&gt;;
    /// IAuditTracker tracker = creator.CreateAudit();
    /// </example>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ICreateAudit<out TAudit> where TAudit : class, IAuditTracker
    {
        /// <summary>
        /// Creates a new instance of an Audit Entity based off the
        /// Current values in the instance of the live entity.
        /// </summary>
        /// <returns>Not Null</returns>
        /// <remarks>
        /// Returning null will cause an internal exception
        /// with in the underlining Audit Engine.
        /// </remarks>
        TAudit CreateAudit();
    }
}