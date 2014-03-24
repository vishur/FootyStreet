
namespace Framework.Entities
{
    /// <summary>
    /// This interface defines the the type 2 or type 3 tables
    /// range continuity. Type 2 tables maintains the sequential
    /// (effectiveBegindate=EffectiveEndDate+1) and type 3 table
    /// maintains the continuous.
    /// </summary>
    public enum RangeContinuity
    {
        Sequential,
        Continuous
    }
}
