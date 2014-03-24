using FootyStreet.Business.Common;
using FootyStreet.Business.Product.Contracts;
using FootyStreet.Utilities;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootyStreet.Business.Product
{
    public class ProductProcessor : BusinessObjectBase,IProduct
    {
        public ProductProcessor(IServiceLocator Container, ISessionContainer DataContainer)
            : base(Container, DataContainer)
        {
              
        }
        public TModel GetNewBusinessObject<TModel>()
          where TModel : new()
        {
            return new TModel();
        }
    }
}
