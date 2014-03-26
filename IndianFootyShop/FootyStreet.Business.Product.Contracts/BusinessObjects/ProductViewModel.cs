using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FootyStreet.Business.Product.Contracts
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductNumber { get; set; }
        public string ProductDescription { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedCategoryDescription { get; set; }
        public string SelectedSubCategoryDescription { get; set; }
        public string SelectedSubCategory { get; set; }
        public string SelectedColor { get; set; }
        public string SelectedUnitofMeasure { get; set; }
        public string SelectedVendorID { get; set; }
        public string SelectedTags { get; set; }


        public decimal BasePrice { get; set; }
        public decimal CostofLabor { get; set; }
        public decimal Overhead { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal SellingPrice { get; set; }

        public List<Category> Categories { get; set; }
        public List<SubCategory> SubCategories { get; set; }
        public List<CategorySubCategory> CategorySubCategories { get; set; }
        public List<UnitOfMeasure> UnitOfMeasure { get; set; }
        public List<Color> Colors { get; set; }
        public List<UomColor> UomColors { get; set; }
        public List<Image> Images { get; set; }
        public List<Vendor> Vendors { get; set; }
        public List<Tag> Tags { get; set; }


        public IEnumerable<HttpPostedFileBase> ThumbnailImages { get; set; }
        public IEnumerable<HttpPostedFileBase> CompleteImages { get; set; }
        public SelectList GetSubCategories()
        {
            IEnumerable<SelectListItem> subcategoryList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                int categoryId = Convert.ToInt32(SelectedCategory);
                subcategoryList = (from s in SubCategories join csc in CategorySubCategories on s.SubCategoryID equals csc.SubCategoryID where s.SubCategoryID == categoryId select s).AsEnumerable().Select(s => new SelectListItem() { Value = s.SubCategoryID.ToString(),Text=s.SubCategoryDescription});
            }
            return new SelectList(subcategoryList, "Value", "Text", SelectedSubCategory);
        }

    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryDescription { get; set; }
    }
    public class SubCategory
    {
        public int SubCategoryID { get; set; }
        public string SubCategoryDescription { get; set; }
    }
    public class CategorySubCategory
    {
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
    }
    public class UnitOfMeasure
    {
        public int UomID { get; set; }
        public string UomDescription { get; set; }
    }

    public class Color
    {
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public string ColorHexCode { get; set; }
        public string ColorRgbCode { get; set; }
        public string ColorDescription { get; set; }
        
    }

    public class UomColor
    {
        public int UomID { get; set; }
        public string UomDescription { get; set; }
        public int ColorID { get; set; }
        public string ColorDescription { get; set; }
        public int Quantity { get; set; }
        
    }
    public class Image
    {
        public int ImageID { get; set; }
        public string ImageAltText { get; set; }
        public string ImageFileName { get; set; }
        public string ImageThumbnailFileName { get; set; }
        public string ImageFilePath { get; set; }
        public string ImageType { get; set; }
        public string ImageThumbnailFilePath { get; set; }
    }
    public class Vendor
    {
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorDescription { get; set; }
        public int AddressID { get; set; }
        public int ContactID { get; set; }
        public string Comments { get; set; }
    }

    public class Tag
    {
        public int TagID { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }
        public int CategoryID { get; set; }
    }

}
