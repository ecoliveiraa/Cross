using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string? NIF { get; set; }  // ← Nullable jsut mandatory if Country is Portugal
        
        [Required]
        public string Address { get; set; }
        
        [Required]
        public string Country { get; set; }
        
        public string Status { get; set; } = "Draft";
        
        [Required]
        public string Stakeholder { get; set; }
        
        [Required]
        public string Contact { get; set; }
    }
}