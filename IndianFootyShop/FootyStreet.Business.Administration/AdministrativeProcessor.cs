using FootyStreet.Business.Common;
using FootyStreet.Utilities;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using FootyStreet.Data;
using Framework;
using FootyStreet.Business.Administration.Contracts;
using FootyStreet.Business.Product.Contracts;
using System.Collections.Generic;
using AutoMapper;
namespace FootyStreet.Business.Administration
{
    public class AdministrativeProcessor : BusinessObjectBase,IAdministrative
    {
        public AdministrativeProcessor(IServiceLocator Container, ISessionContainer DataContainer)
            : base(Container, DataContainer)
        {
              
        }
        public TModel GetNewBusinessObject<TModel>()
          where TModel : new()
        {
            return new TModel();
        }

        public const string ProductViewModelDataKey = "ProductViewModelDataKey";
        

        public ProductViewModel ProductViewModelData
        {
            get
            {
                ProductViewModel productViewModelData = GetBuinessObjects<ProductViewModel>(ProductViewModelDataKey);
                return productViewModelData;
            }
            set
            {
                SetBusinessObject<ProductViewModel>(value, ProductViewModelDataKey);
            }
        }


        public bool insert()
        {
            var objTag = RepositoryFactory.GetRepository<Tag>();
            var tag = objTag.Create();
            tag.TagName = "First";
            tag.TagDescription = "Desc";
            tag.UpdatedDate = System.DateTime.Now;
            tag.CreatedDate = System.DateTime.Now;
            tag.UpdatedBy = "Admin";
            tag.CreatedBy = "Admin";
            objTag.Insert(tag);
            objTag.Save();
            var t = tag.TagID;
            return true;
        }

        public ProductViewModel GetProductMasterData()
        {
            var productViewModel = new ProductViewModel
            {
                Categories = new List<Product.Contracts.Category>(),
                SubCategories = new List<Product.Contracts.SubCategory>(),
                Colors = new List<Product.Contracts.Color>(),
                Images = new List<Product.Contracts.Image>(),
                UnitOfMeasure = new List<UnitOfMeasure>(),
                UomColors = new List<UomColor>(),
                CategorySubCategories = new List<Product.Contracts.CategorySubCategory>(),
                Vendors = new List<Product.Contracts.Vendor>()
            };
            productViewModel.Categories.AddRange(RepositoryFactory.GetRepository<FootyStreet.Data.Category>().Data.ToList().Select(m => new Product.Contracts.Category
            {
                CategoryID = m.CategoryID,
                CategoryDescription = m.CategoryName
            }));


            productViewModel.SubCategories.AddRange(RepositoryFactory.GetRepository<FootyStreet.Data.SubCategory>().Data.ToList().Select(m => new Product.Contracts.SubCategory
            {
                SubCategoryID = m.SubCategoryID,
                SubCategoryDescription = m.SubCategoryName
            }));

            productViewModel.CategorySubCategories.AddRange(RepositoryFactory.GetRepository<FootyStreet.Data.CategorySubCategory>().Data.ToList().Select(m => new Product.Contracts.CategorySubCategory
            {
                CategoryID = m.CategoryID,
                SubCategoryID = m.SubCategoryID
            }));

            productViewModel.Colors.AddRange(RepositoryFactory.GetRepository<FootyStreet.Data.Color>().Data.ToList().Select(m => new Product.Contracts.Color
            {
                ColorID = m.ColorID,
                ColorDescription = m.ColorDescription,
                ColorHexCode = m.ColorHexCode,
                ColorRgbCode = m.ColorRGBCode,
                ColorName = m.ColorName
            }));

            productViewModel.UnitOfMeasure.AddRange(RepositoryFactory.GetRepository<FootyStreet.Data.UnitofMeasure>().Data.ToList().Select(m => new Product.Contracts.UnitOfMeasure
            {
                UomID = m.UOMID,
                UomDescription = m.UOMDescription
               
            }));

            productViewModel.Vendors.AddRange(RepositoryFactory.GetRepository<FootyStreet.Data.Vendor>().Data.ToList().Select(m => new Product.Contracts.Vendor
            {
                VendorID = m.VendorID,
                VendorName = m.VendorName,
                VendorDescription = m.VendorDescription,
                AddressID = m.AddressID,
                Comments = m.Comments,
                ContactID = m.ContactID
            }));
           
            ProductViewModelData = productViewModel;
            return productViewModel;
        }


        public List<Product.Contracts.SubCategory> GetSubCategories(int categoryId)
        {
            var subCategories = (from s in ProductViewModelData.SubCategories join csc in ProductViewModelData.CategorySubCategories on s.SubCategoryID equals csc.SubCategoryID where csc.CategoryID == categoryId select s).ToList();
            return subCategories;
                                  
        }

        public int SaveProduct(ProductViewModel productViewModel)
        {
            var createdBy = "Admin";
            var updatedBy = "Admin";
            var updatedDate = System.DateTime.Now;
            var createdDate = System.DateTime.Now;
            productViewModel.Images = new List<Product.Contracts.Image>();

            var relativeOriginalImagePath = "ProductImages/" + productViewModel.SelectedCategoryDescription + "/" + productViewModel.SelectedSubCategoryDescription + "/" + productViewModel.ProductNumber + "/OriginalImages/";
            var relativeThumbnailImagePath = "ProductImages/" + productViewModel.SelectedCategoryDescription + "/" + productViewModel.SelectedSubCategoryDescription + "/" + productViewModel.ProductNumber + "/ThumbnailImages/";

            var originalImagePath = AppDomain.CurrentDomain.BaseDirectory + relativeOriginalImagePath;
            var thumbnailImagePath = AppDomain.CurrentDomain.BaseDirectory + relativeThumbnailImagePath;

            if (!System.IO.Directory.Exists(originalImagePath))
            {
                System.IO.Directory.CreateDirectory(originalImagePath);
            }
            if (!System.IO.Directory.Exists(thumbnailImagePath))
            {
                System.IO.Directory.CreateDirectory(thumbnailImagePath);
            }

            foreach (var image in productViewModel.CompleteImages)
            {
                image.SaveAs(originalImagePath + image.FileName);

                productViewModel.Images.Add(new Product.Contracts.Image
                {
                    ImageAltText = productViewModel.ProductName,
                    ImageFileName = image.FileName,
                    ImageThumbnailFileName = image.FileName,
                    ImageFilePath = relativeOriginalImagePath,
                    ImageThumbnailFilePath = relativeThumbnailImagePath

                });
            }
            foreach (var image in productViewModel.ThumbnailImages)
            {
                image.SaveAs(thumbnailImagePath + image.FileName);

                productViewModel.Images.Add(new Product.Contracts.Image
                {
                    ImageAltText = productViewModel.ProductName,
                    ImageFileName = image.FileName,
                    ImageThumbnailFileName = image.FileName,
                    ImageFilePath = relativeOriginalImagePath,
                    ImageThumbnailFilePath = relativeThumbnailImagePath

                });
            }

            using (var unitofwork = RepositoryFactory.BeginUnitOfWork())
            {
                var productRepository = RepositoryFactory.GetRepository<FootyStreet.Data.Product>().Create();
                var productDetailRepository = RepositoryFactory.GetRepository<FootyStreet.Data.ProductDetail>().Create();
                var productCategorySubCategoryRepository = RepositoryFactory.GetRepository<FootyStreet.Data.ProductCategorySubCategory>().Create();
                var productUomColorRepository = RepositoryFactory.GetRepository<FootyStreet.Data.ProductUOMColor>().Create();
                var imageRepository = RepositoryFactory.GetRepository<FootyStreet.Data.Image>().Create();
                var productImageRepository = RepositoryFactory.GetRepository<FootyStreet.Data.ProductImage>().Create();
                var productInventory = RepositoryFactory.GetRepository<FootyStreet.Data.ProductInventory>().Create();
                var priceRepository = RepositoryFactory.GetRepository<FootyStreet.Data.Price>().Create();

                //Images

                foreach (var image in productViewModel.Images)
                {
                    imageRepository.CreatedBy = createdBy;
                    imageRepository.CreatedDate = createdDate;
                    imageRepository.UpdatedBy = updatedBy;
                    imageRepository.UpdatedDate = updatedDate;
                    imageRepository.ImageAltText = image.ImageAltText;
                    imageRepository.ImageFileName = image.ImageFileName;
                    imageRepository.ImageFilePath = image.ImageFilePath;
                    imageRepository.ImageThumbnailFileName = image.ImageThumbnailFileName;
                    imageRepository.ImageThumbnailFilePath = image.ImageThumbnailFilePath;

                    productImageRepository.Image = imageRepository;
                    productRepository.ProductImages.Add(productImageRepository);
                }

                //Price
                priceRepository.BasePrice = productViewModel.BasePrice;
                priceRepository.CostofLabor = productViewModel.CostofLabor;
                priceRepository.Overhead = productViewModel.Overhead;
                priceRepository.ProfitMargin = productViewModel.ProfitMargin;
                priceRepository.SellingPrice = productViewModel.SellingPrice;

                productDetailRepository.Price = priceRepository;

                //ProductDetail
                productDetailRepository.ProductDescription = productViewModel.ProductDescription;
                productDetailRepository.ProductMake = "Nike";
                productDetailRepository.ProductName = productViewModel.ProductName;
                productDetailRepository.ProductVendorID = Convert.ToInt32(productViewModel.SelectedVendorID);
                productDetailRepository.CreatedBy = createdBy;
                productDetailRepository.CreatedDate = createdDate;
                productDetailRepository.UpdatedBy = updatedBy;
                productDetailRepository.UpdatedDate = updatedDate;

                productRepository.ProductDetail = productDetailRepository;

                productRepository.ProductBeginDate = createdDate;
                productRepository.ProductNumber = productViewModel.ProductNumber;
              //  productRepository.ProductUOMColors = new ICollection<FootyStreet.Data.ProductUOMColor>();

                foreach (var productuomcolor in productViewModel.UomColors)
                {
                    productUomColorRepository.UOMID = productuomcolor.UomID;
                    productUomColorRepository.ColorID = productuomcolor.ColorID;
                    productInventory.AvailableStock = productuomcolor.Quantity;
                    
                    productInventory.CreatedBy = createdBy;
                    productInventory.CreatedDate = createdDate;
                    productInventory.UpdatedBy = updatedBy;
                    productInventory.UpdatedDate = updatedDate;


                    productUomColorRepository.ProductInventories.Add(productInventory);

                    productUomColorRepository.CreatedBy = createdBy;
                    productUomColorRepository.CreatedDate = createdDate;
                    productUomColorRepository.UpdatedBy = updatedBy;
                    productUomColorRepository.UpdatedDate = updatedDate;


                    productRepository.ProductUOMColors.Add(productUomColorRepository);
                }
                productCategorySubCategoryRepository.CategorySubCategoryID = productViewModel.CategorySubCategories.Where(m => m.SubCategoryID == Convert.ToInt32(productViewModel.SelectedSubCategory)).FirstOrDefault().SubCategoryID;
                
                    productCategorySubCategoryRepository.CreatedBy = createdBy;
                    productCategorySubCategoryRepository.CreatedDate = createdDate;
                    productCategorySubCategoryRepository.UpdatedBy = updatedBy;
                    productCategorySubCategoryRepository.UpdatedDate = updatedDate;

                    productRepository.ProductCategorySubCategories.Add(productCategorySubCategoryRepository);

                    var product = RepositoryFactory.GetRepository<FootyStreet.Data.Product>();
                    product.Insert(productRepository, true);
                    //unitofwork.Save();
            }

            return 0;
        }
    }
}
