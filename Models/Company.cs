namespace WebApplication1.Models
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NIF { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Status { get; set; } = "Draft";
        public string Stakeholder { get; set; }
        public string Contact { get; set; }
    }
}
