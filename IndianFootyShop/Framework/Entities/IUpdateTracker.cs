using System;
using System.ComponentModel;

namespace Framework.Entities
{
    /// <summary>
    /// Interface for Tracking Update Information.
    /// Using this in conjunction with EntityRepository,
    /// will populate the data during Update of the PrimaryEntity
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IUpdateTracker
    {
        string UpdatedBy { get; set; }

        DateTime UpdatedDate { get; set; }
    }
}