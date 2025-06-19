using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Proposal
    {
        public Guid ProposalID { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid LeadID { get; set; }
        
        public List<Guid> ProductIDs { get; set; } = new();
        
        public decimal? ProductionCost { get; set; }      // ← Nullable - just mandatory if !status="Draft"
        
        public int? MonthlyProducedProducts { get; set; } // ← Nullable - just mandatory if status="Finalized"
        
        public decimal ExpectedMonthlyProfit { get; set; }
        
        public string Status { get; set; } = "Draft";
    }
}