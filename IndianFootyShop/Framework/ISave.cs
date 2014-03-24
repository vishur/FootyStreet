namespace Framework
{
    /// <summary>
    /// An interface that saves a unit of work
    /// (without the need of DbContext or TEntity as a generic)
    /// </summary>
    internal interface ISave
    {
        /// <summary>
        /// Saves any changes.
        /// </summary>
        void Save();
    }
}