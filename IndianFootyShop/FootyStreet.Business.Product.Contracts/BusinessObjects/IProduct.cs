using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootyStreet.Business.Product.Contracts
{
    public interface IProduct
    {
        TModel GetNewBusinessObject<TModel>()
         where TModel : new();
    }
}
