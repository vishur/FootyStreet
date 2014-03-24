using System;
using System.ComponentModel;

namespace Framework.Entities
{
    /// <summary>
    /// Interface for Tracking Audit Information.
    /// Using this in conjunction with EntityRepository,
    /// will populate the data during Insert of the SecondaryEntity
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IAuditTracker
    {
        string AuditUserId { get; set; }

        DateTime AuditDate { get; set; }

        bool  IsDeleted { get; set; }
    }
}