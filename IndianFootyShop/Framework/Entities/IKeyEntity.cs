using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Entities
{

    /// <summary>
    /// Interface for updating keyentity Information.
    /// whenever a type 2 or type 3 entity gets updated or inserted
    /// the data must be validated against the key entity, as the key entity
    /// would help to maintain temporial referential integrity.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IKeyEntity
    {
        DateTime? EarliestEffectiveDate { get; set; }

        DateTime? LatestEffectiveBeginDate { get; set; }

        DateTime? LatestEffectiveDate { get; set; }

        IEnumerable<IDateRange> Children { get;}
    }
}
