using System;
using System.ComponentModel;

namespace Framework.Entities
{
    /// <summary>
    /// Interface for Tracking Insert Information.
    /// Using this in conjunction with EntityRepository,
    /// will populate the data during Insertion of the PrimaryEntity
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInsertTracker
    {
        string CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }
    }
}