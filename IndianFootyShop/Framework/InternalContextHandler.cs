using System.Data.Entity;

namespace Framework
{
    internal sealed class InternalContextHandler<TDbContext> : LazyContextHandler<TDbContext>, ISave where TDbContext : DbContext, new()
    {
        //Base class is abstract and no public constructor.
        
        void ISave.Save()
        {
            this.Save();
        }
    }
}