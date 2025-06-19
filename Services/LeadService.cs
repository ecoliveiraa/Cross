using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICompanyService _companyService;

        public LeadService(ILeadRepository leadRepository, ICompanyService companyService)
        {
            _leadRepository = leadRepository;
            _companyService = companyService;
        }

        public async Task<Lead> CreateAsync(Lead lead)
        {
            // Validação de negócio: verificar se a company existe
            var company = await _companyService.GetByIdAsync(lead.CompanyId);
            if (company == null)
                return null; // Ou lançar exception

            // Lógica de negócio: definir Country baseado na Company
            lead.Country = company.Country;

            // Garantir que tem ID
            if (lead.LeadID == Guid.Empty)
                lead.LeadID = Guid.NewGuid();

            return await _leadRepository.CreateAsync(lead);
        }

        public async Task<List<Lead>> GetAllAsync()
        {
            return await _leadRepository.GetAllAsync();
        }

        public async Task<Lead> GetByIdAsync(Guid id)
        {
            return await _leadRepository.GetByIdAsync(id);
        }

        public async Task<Lead> UpdateAsync(Lead lead)
        {
            // Validação se existe
            var existing = await _leadRepository.GetByIdAsync(lead.LeadID);
            if (existing == null)
                return null;

            return await _leadRepository.UpdateAsync(lead);
        }
    }
}