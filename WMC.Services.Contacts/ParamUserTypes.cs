//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMC.Services.Contacts
{
    using System;
    using System.Collections.Generic;
    
    public partial class ParamUserTypes
    {
        public int UserTypeId { get; set; }
        public string UserType { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public int CreateUserId { get; set; }
        public Nullable<int> UpdateUserId { get; set; }
        public short Priority { get; set; }
        public byte[] ts { get; set; }
    }
}
