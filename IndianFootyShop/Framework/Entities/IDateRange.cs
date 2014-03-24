using Framework.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Framework.Entities
{
    /// <summary>
    /// When a type 2 or type 3 tables gets inserted/updated 
    /// the effective begin date and effective end date must have
    /// been validated to prevent overlaps against the records in 
    /// the table already for the specific key.  IDateRage interface
    /// helps to perform all the validation to prevent the overlaps.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IDateRange
    {
        DateTime EffectiveBeginDate { get; set; }

        DateTime? EffectiveEndDate { get; set; }

        IEnumerable<IDateRange> Siblings { get; }

        RangeContinuity Continuity { get; }

        IKeyEntity KeyEntity { get; }
    }
}