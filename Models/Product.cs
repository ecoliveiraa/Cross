namespace WebApplication1.Models
{  public class Product
  {
      public Guid ProductID { get; set; } = Guid.NewGuid();
      public Guid? DependentProductId { get; set; }
      public string ProductType { get; set; }
  }
}