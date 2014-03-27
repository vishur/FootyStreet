using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public interface IMap<TDestination>
    {
        void Map(TDestination destination);
    }
}
