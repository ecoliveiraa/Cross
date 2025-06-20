using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Lead
    {
        public Guid LeadID { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid CompanyId { get; set; }
        
       // public string Country { get; set; } -> Don't needed, use Company.Country reference
        
        public string? BusinessType { get; set; }  // ← Nullable just mandatory is status = "Active"
        public string Status { get; set; } = "Draft";

    }
}