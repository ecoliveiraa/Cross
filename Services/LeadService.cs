using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using WebApplication1.Validation;

namespace WebApplication1.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICompanyService _companyService;
        private readonly IRuleManager _ruleManager;

        public LeadService(ILeadRepository leadRepository, ICompanyService companyService, IRuleManager ruleManager)
        {
            _leadRepository = leadRepository;
            _companyService = companyService;
            _ruleManager = ruleManager;
        }

        public async Task<Lead> CreateAsync(Lead lead)
        {
            // VALIDAÇÃO DINÂMICA
            var validationResult = _ruleManager.Validate(lead);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            // Verify if the company exists
            var company = await _companyService.GetByIdAsync(lead.CompanyId);
            if (company == null)
                return null;

            
            // lead.Country = company.Country; -> Not needed, use Company.Country reference

           
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
            // VALIDAÇÃO DINÂMICA também no UPDATE
            var validationResult = _ruleManager.Validate(lead);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            // Validação se existe
            var existing = await _leadRepository.GetByIdAsync(lead.LeadID);
            if (existing == null)
                return null;

            return await _leadRepository.UpdateAsync(lead);
        }
    }
}