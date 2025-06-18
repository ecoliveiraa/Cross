using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.InMemory
{
    public class InMemoryProposalService : IProposalService
    {
        private readonly List<Proposal> _proposals = new();
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly ILeadService _leadService;

        public InMemoryProposalService(IProductService productService, ICompanyService companyService, ILeadService leadService)
        {
            _productService = productService;
            _companyService = companyService;
            _leadService = leadService;
        }

        public Task<Proposal> CreateAsync(Proposal proposal)
        {
            _proposals.Add(proposal);
            return Task.FromResult(proposal);
        }

        public Task<Proposal> GetByIdAsync(Guid id) =>
            Task.FromResult(_proposals.FirstOrDefault(p => p.ProposalID == id));

        public Task<List<Proposal>> GetAllAsync() => Task.FromResult(_proposals);

        public async Task AddProductAsync(Guid proposalId, Guid productId)
        {
            var proposal = await GetByIdAsync(proposalId);
            var product = await _productService.GetByIdAsync(productId);

            if (proposal == null || product == null)
                throw new Exception("Proposal or Product not found");


            // Check if the product is compatible with the proposal's lead
            // Check if the product needs a dependent product
            if (product.DependentProductId.HasValue)
            {
              //TODO: fazer com que o produto dependente seja adicionado automaticamente
                var required = await _productService.GetByIdAsync(product.DependentProductId.Value);
                if (required == null || !proposal.ProductIDs.Contains(required.ProductID))
                    throw new Exception("Missing required dependent product");

                if (required.ProductType != product.ProductType)
                    throw new Exception("Dependent product type mismatch");
            }


            // Check if the product is already added
            if (!proposal.ProductIDs.Contains(productId))
                proposal.ProductIDs.Add(productId);
        }

        public Task<Proposal> UpdateAsync(Proposal proposal)
        {
            var existing = _proposals.FirstOrDefault(p => p.ProposalID == proposal.ProposalID);
            if (existing != null)
            {
                existing.LeadID = proposal.LeadID;
                existing.ProductIDs = proposal.ProductIDs;
                existing.ProductionCost = proposal.ProductionCost;
                existing.MonthlyProducedProducts = proposal.MonthlyProducedProducts;
                existing.ExpectedMonthlyProfit = proposal.ExpectedMonthlyProfit;
                existing.Status = proposal.Status;
            }
            return Task.FromResult(existing);
        }

        public async Task FinalizeProposalAsync(Guid proposalId)
        {
            var proposal = await GetByIdAsync(proposalId);
            if (proposal == null)
                throw new Exception("Proposal not found");

            proposal.Status = "Finalized";

            var lead = await _leadService.GetLeadByIdAsync(proposal.LeadID);
            if (lead == null)
                throw new Exception("Associated lead not found");

            var company = await _companyService.GetByIdAsync(lead.CompanyId);
            if (company != null)
            {
                company.Status = "Active";
                await _companyService.UpdateAsync(company);
            }
        }
    }
}
