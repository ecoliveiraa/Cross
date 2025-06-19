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
            // VALIDAÇÃO DINÂMICA
            var validationResult = _ruleManager.Validate(proposal);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            // Validação: verificar se o Lead existe
            var lead = await _leadService.GetByIdAsync(proposal.LeadID);
            if (lead == null)
                return null;

            // Garantir que tem ID
            if (proposal.ProposalID == Guid.Empty)
                proposal.ProposalID = Guid.NewGuid();

            var created = await _proposalRepository.CreateAsync(proposal);

            // Lógica de negócio: atualizar status do Lead para "Finalized"
            if (created != null)
            {
                lead.Status = "Finalized";
                await _leadService.UpdateAsync(lead);
            }

            return created;
        }

        public async Task<Proposal> GetByIdAsync(Guid id)
        {
            return await _proposalRepository.GetByIdAsync(id);
        }

        public async Task<List<Proposal>> GetAllAsync()
        {
            return await _proposalRepository.GetAllAsync();
        }

        public async Task<Proposal> UpdateAsync(Proposal proposal)
        {
            // VALIDAÇÃO DINÂMICA também no UPDATE
            var validationResult = _ruleManager.Validate(proposal);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            // Validação se existe
            var existing = await _proposalRepository.GetByIdAsync(proposal.ProposalID);
            if (existing == null)
                return null;

            return await _proposalRepository.UpdateAsync(proposal);
        }

        public async Task AddProductAsync(Guid proposalId, Guid productId)
        {
            var proposal = await _proposalRepository.GetByIdAsync(proposalId);
            var product = await _productService.GetByIdAsync(productId);

            if (proposal == null || product == null)
                throw new Exception("Proposal or Product not found");

            // Validação: verificar se produto dependente existe
            if (product.DependentProductId.HasValue)
            {
                var requiredProduct = await _productService.GetByIdAsync(product.DependentProductId.Value);
                if (requiredProduct == null || !proposal.ProductIDs.Contains(requiredProduct.ProductID))
                    throw new Exception("Missing required dependent product");

                if (requiredProduct.ProductType != product.ProductType)
                    throw new Exception("Dependent product type mismatch");
            }

            // Adicionar produto se ainda não existe
            if (!proposal.ProductIDs.Contains(productId))
            {
                proposal.ProductIDs.Add(productId);
                await _proposalRepository.UpdateAsync(proposal);
            }
        }

        public async Task FinalizeProposalAsync(Guid proposalId)
        {
            var proposal = await _proposalRepository.GetByIdAsync(proposalId);
            if (proposal == null)
                throw new Exception("Proposal not found");

            // Atualizar status da proposal
            proposal.Status = "Finalized";
            
            // VALIDAÇÃO DINÂMICA antes de finalizar
            var validationResult = _ruleManager.Validate(proposal);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Cannot finalize proposal: {string.Join(", ", validationResult.Errors)}");
            }

            await _proposalRepository.UpdateAsync(proposal);

            // Atualizar status da company para "Active"
            var lead = await _leadService.GetByIdAsync(proposal.LeadID);
            if (lead != null)
            {
                var company = await _companyService.GetByIdAsync(lead.CompanyId);
                if (company != null)
                {
                    company.Status = "Active";
                    await _companyService.UpdateAsync(company);
                }
            }
        }
    }
}