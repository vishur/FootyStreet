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
    public partial class AddressType : IInsertTracker, IUpdateTracker
    {
        public AddressType()
        {
            this.Addresses = new HashSet<Address>();
        }
    
        public int AddressTypeID { get; set; }
        public string AddressTypeDesc { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual ICollection<Address> Addresses { get; set; }
    
    }
}