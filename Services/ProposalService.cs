using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class ProposalService : IProposalService
    {
        private readonly IProposalRepository _proposalRepository;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly ILeadService _leadService;

        public ProposalService(
            IProposalRepository proposalRepository,
            IProductService productService, 
            ICompanyService companyService, 
            ILeadService leadService)
        {
            _proposalRepository = proposalRepository;
            _productService = productService;
            _companyService = companyService;
            _leadService = leadService;
        }

        public async Task<Proposal> CreateAsync(Proposal proposal)
        {
            // Validação: verificar se o Lead existe
            var lead = await _leadService.GetByIdAsync(proposal.LeadID);
            if (lead == null)
                return null;

            // Lógica de negócio: preencher dados do Lead na Proposal
            // (assumindo que o modelo Proposal tem estes campos - se não tiver, remove)
            // proposal.CompanyId = lead.CompanyId;
            // proposal.Country = lead.Country;
            // proposal.BusinessType = lead.BusinessType;

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