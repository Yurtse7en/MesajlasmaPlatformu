//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MesajlasmaPlatformu.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Mesaj
    {
        public int messageId { get; set; }
        public int senderId { get; set; }
        public int recipientId { get; set; }
        public Nullable<int> grupmesajId { get; set; }
        public string content { get; set; }
        public string mesajtarih { get; set; }
    
        public virtual Grup Grup { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
