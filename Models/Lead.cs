namespace WebApplication1.Models
{
    public class Lead
    {
      public Guid LeadID { get; set; } = Guid.NewGuid(); 
      public Guid CompanyId { get; set; } 
      public string Country { get; set; } 
      public string BusinessType { get; set; } 
      public string Status { get; set; } = "Draft"; 
    }
}
