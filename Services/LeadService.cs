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
            if (lead == null)
                throw new ValidationException("Lead data is required.");

          // Ignore LeadID from client, generate new one | double validation
            lead.LeadID = Guid.NewGuid();

            // Verify if the company exists - vai lançar NotFoundException se não existir
            var company = await _companyService.GetByIdAsync(lead.CompanyId);

            // Dynamic validation using IRuleManager - ValidationException = 400 Bad Request
            var validationResult = _ruleManager.Validate(lead);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            //Dont duplicate Country because using Country from Company reference

            return await _leadRepository.CreateAsync(lead);
        }

        public async Task<List<Lead>> GetAllAsync()
        {
            return await _leadRepository.GetAllAsync();
        }

        public async Task<Lead> GetByIdAsync(Guid id)
        {
            var lead = await _leadRepository.GetByIdAsync(id);
            if (lead == null)
                throw new NotFoundException($"Lead with ID {id} not found.");
            
            return lead;
        }

        public async Task<Lead> UpdateAsync(Lead lead)
        {
            if (lead == null)
                throw new ValidationException("Lead data is required.");

            // Verify if lead exists before validations
            var existing = await _leadRepository.GetByIdAsync(lead.LeadID);
            if (existing == null)
                throw new NotFoundException($"Lead with ID {lead.LeadID} not found.");

            // Dynamic validation using IRuleManager on Update too
            var validationResult = _ruleManager.Validate(lead);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            return await _leadRepository.UpdateAsync(lead);
        }
    }
}