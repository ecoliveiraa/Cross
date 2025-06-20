using WebApplication1.Models;
using WebApplication1.Validation;

namespace WebApplication1.Configuration
{
    public static class RuleConfiguration
    {
        public static void ConfigureRules(IRuleManager ruleManager)
        {
            // COMPANY RULES
            ruleManager.SetRequired<Company>("NIF", 
                company => company.Country == "Portugal", 
                true, 
                "NIF is required for Portuguese companies");

            // LEAD RULES
            ruleManager.SetRequired<Lead>("BusinessType", 
                lead => lead.Status == "Active", 
                true, 
                "BusinessType is required when lead is Active");

            //Propopsal RULES
            ruleManager.SetRequired<Proposal>("ProductionCost", 
                proposal => proposal.Status != "Draft", 
                true, 
                "ProductionCost is required for non-draft proposals");

            ruleManager.SetRequired<Proposal>("MonthlyProducedProducts", 
                proposal => proposal.Status == "Finalized", 
                true, 
                "MonthlyProducedProducts is required for finalized proposals");
            ruleManager.SetRequired<Proposal>("ProductIDs", 
                proposal => proposal.Status == "Finalized", 
                true, 
                "At least one product is required to finalize proposal");

        }
    }
}
