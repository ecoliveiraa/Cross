using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using WebApplication1.Validation;

namespace WebApplication1.Services
{
    public class ProposalService : IProposalService
    {
        private readonly IProposalRepository _proposalRepository;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly ILeadService _leadService;
        private readonly IRuleManager _ruleManager;

        public ProposalService(
            IProposalRepository proposalRepository,
            IProductService productService, 
            ICompanyService companyService, 
            ILeadService leadService,
            IRuleManager ruleManager)
        {
            _proposalRepository = proposalRepository;
            _productService = productService;
            _companyService = companyService;
            _leadService = leadService;
            _ruleManager = ruleManager;
        }

        public async Task<Proposal> CreateAsync(Proposal proposal)
        {
            if (proposal == null)
                throw new ValidationException("Proposal data is required.");

            //Ignore ProposalID from client, generate new one | double validation
            proposal.ProposalID = Guid.NewGuid();

            
            var lead = await _leadService.GetByIdAsync(proposal.LeadID);

            //dinamic validation using IRuleManager - ValidationException = 400 Bad Request
            var validationResult = _ruleManager.Validate(proposal);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            var created = await _proposalRepository.CreateAsync(proposal);

            //Business logic: update lead status to "Finalized" after proposal creation
            lead.Status = "Finalized";
            await _leadService.UpdateAsync(lead);

            return created;
        }

        public async Task<Proposal> GetByIdAsync(Guid id)
        {
            var proposal = await _proposalRepository.GetByIdAsync(id);
            if (proposal == null)
                throw new NotFoundException($"Proposal with ID {id} not found.");
            
            return proposal;
        }

        public async Task<List<Proposal>> GetAllAsync()
        {
            return await _proposalRepository.GetAllAsync();
        }

        public async Task<Proposal> UpdateAsync(Proposal proposal)
        {
            if (proposal == null)
                throw new ValidationException("Proposal data is required.");

            // Verify if proposal exists before validations
            var existing = await _proposalRepository.GetByIdAsync(proposal.ProposalID);
            if (existing == null)
                throw new NotFoundException($"Proposal with ID {proposal.ProposalID} not found.");

            // Dynamic validation using IRuleManager on Update too
            var validationResult = _ruleManager.Validate(proposal);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            return await _proposalRepository.UpdateAsync(proposal);
        }

        public async Task AddProductAsync(Guid proposalId, Guid productId)
        {
           // GEtByIdAsync will throw NotFoundException if it does not exist
            var proposal = await GetByIdAsync(proposalId);
            var product = await _productService.GetByIdAsync(productId);

            //Verify if product already exists
            if (proposal.ProductIDs.Contains(productId))
                throw new ValidationException("Product already exists in this proposal.");

           //Verify if dependent product exists
            if (product.DependentProductId.HasValue) 
            {
                var requiredProduct = await _productService.GetByIdAsync(product.DependentProductId.Value);
                if (!proposal.ProductIDs.Contains(requiredProduct.ProductID))
                    throw new ValidationException("Missing required dependent product. Add the dependent product first.");

                if (requiredProduct.ProductType != product.ProductType)
                    throw new ValidationException("Dependent product type mismatch.");
            }

            // Add product to proposal
            proposal.ProductIDs.Add(productId);
            await _proposalRepository.UpdateAsync(proposal);
        }

        public async Task FinalizeProposalAsync(Guid proposalId)
        {
            
            var proposal = await GetByIdAsync(proposalId);

            //Make a copy of the proposal for validation
            var proposalForValidation = new Proposal
            {
                ProposalID = proposal.ProposalID,
                LeadID = proposal.LeadID,
                ProductIDs = proposal.ProductIDs,
                ProductionCost = proposal.ProductionCost,
                MonthlyProducedProducts = proposal.MonthlyProducedProducts,
                ExpectedMonthlyProfit = proposal.ExpectedMonthlyProfit,
                Status = "Finalized" // Status for validation
            };

            // Dynamic validation using IRuleManager and Proposal copy
            var validationResult = _ruleManager.Validate(proposalForValidation);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Cannot finalize proposal: {string.Join(", ", validationResult.Errors)}");
            }

            // if proposal copy was valid, proceed with finalization
            try
            {
                // Update proposal status to "Finalized"
                proposal.Status = "Finalized";
                await _proposalRepository.UpdateAsync(proposal);

                //Update company status to "Active"
                var lead = await _leadService.GetByIdAsync(proposal.LeadID);
                var company = await _companyService.GetByIdAsync(lead.CompanyId);
                company.Status = "Active";
                await _companyService.UpdateAsync(company);
            }
            catch (Exception)
            {
                //in the event of an error, it would be ideal to perform a rollback
                //but with inmemory storage, we do not have transactions
                //during production you would use a database transaction
                //throwing a generic exception to indicate failure


                throw new Exception("Failed to finalize proposal. Please try again.");
            }
        }
    }
}