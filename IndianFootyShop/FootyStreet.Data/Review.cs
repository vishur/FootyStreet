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
    public partial class Review : IInsertTracker, IUpdateTracker
    {
        public Review()
        {
            this.ProductReviews = new HashSet<ProductReview>();
        }
    
        public int ReviewID { get; set; }
        public string Review1 { get; set; }
        public int Rating { get; set; }
        public string Reviewer { get; set; }
        public bool IsGenuine { get; set; }
        public bool IsApproved { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
    
    }
}