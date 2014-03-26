using FootyStreet.Business.Administration.Contracts;
using FootyStreet.Business.Product.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IndianFootyShop.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Products()
        {
            return View( GetProductMasterData());
        }

        private ProductViewModel GetProductMasterData()
        {
            var administrativeProcessor = DependencyResolver.Current.GetService<IAdministrative>();
            return administrativeProcessor.GetProductMasterData();
        }

        public JsonResult GetSubCategories(int categoryId = 0)
        {
            var administrativeProcessor = DependencyResolver.Current.GetService<IAdministrative>();
            var subCategoryList = administrativeProcessor.GetSubCategories(categoryId);
            //var tags = administrativeProcessor
            return Json(new SelectList(subCategoryList.ToArray(), "SubCategoryID", "SubCategoryDescription"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayUomColorList(int uomId, int colorId, int quantity, string uomDescription, string colorDescription)
        {
            var administrativeProcessor = DependencyResolver.Current.GetService<IAdministrative>();
            administrativeProcessor.ProductViewModelData.UomColors.Add(new UomColor
            {
                UomID = uomId,
                UomDescription = uomDescription,
                ColorID = colorId,
                ColorDescription = colorDescription,
                Quantity = quantity
            });

            return PartialView("_ProductsPartial", administrativeProcessor.ProductViewModelData.UomColors);
        }

        public ActionResult SaveProducts(HttpPostedFileBase obj)
        {
            var productViewModel = new ProductViewModel();
            TryUpdateModel(productViewModel);
            var administrativeProcessor = DependencyResolver.Current.GetService<IAdministrative>();
            productViewModel.UomColors = new List<UomColor>();
            productViewModel.CategorySubCategories = new List<CategorySubCategory>();
            productViewModel.UomColors = administrativeProcessor.ProductViewModelData.UomColors;
            productViewModel.CategorySubCategories = administrativeProcessor.ProductViewModelData.CategorySubCategories;
            administrativeProcessor.SaveProduct(productViewModel);
            return View(GetProductMasterData());
        }
    }
}
