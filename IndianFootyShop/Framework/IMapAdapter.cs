using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public interface IMapAdapter<in TSource, in TDestination>
    {
        void Map(TSource source, TDestination destination);
    }
}
