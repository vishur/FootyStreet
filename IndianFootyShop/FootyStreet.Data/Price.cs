//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FootyStreet.Data
{
    
    
    
    using System;
    using System.Collections.Generic;
    using Framework.Entities;
    using System.Linq;
    public partial class Price
    {
        public Price()
        {
            this.ProductDetails = new HashSet<ProductDetail>();
        }
    
        public int PriceID { get; set; }
        public decimal BasePrice { get; set; }
        public decimal CostofLabor { get; set; }
        public decimal Overhead { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal SellingPrice { get; set; }
    
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    
    }
}
