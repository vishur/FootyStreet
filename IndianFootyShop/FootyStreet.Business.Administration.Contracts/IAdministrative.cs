using FootyStreet.Business.Product.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootyStreet.Business.Administration.Contracts
{
    public interface IAdministrative
    {
        TModel GetNewBusinessObject<TModel>()
          where TModel : new();
        bool insert();
        int SaveProduct(ProductViewModel productViewModel);
        ProductViewModel ProductViewModelData{get;set;}
        ProductViewModel GetProductMasterData();
        List<Product.Contracts.SubCategory> GetSubCategories(int categoryId);
        
    }

   
}
