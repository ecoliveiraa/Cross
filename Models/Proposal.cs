namespace WebApplication1.Models
{
    public class Proposal
    {
        public Guid ProposalID { get; set; } = Guid.NewGuid();
        public Guid LeadID { get; set; }
        public List<Guid> ProductIDs { get; set; } = new();
        public decimal ProductionCost { get; set; }
        public int MonthlyProducedProducts { get; set; }
        public decimal ExpectedMonthlyProfit { get; set; }
        public string Status { get; set; } = "Draft";
    }
}
