using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using WebApplication1.Validation;

namespace WebApplication1.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IRuleManager _ruleManager;

        public CompanyService(ICompanyRepository companyRepository, IRuleManager ruleManager)
        {
            _companyRepository = companyRepository;
            _ruleManager = ruleManager;
        }

        public async Task<Company> CreateAsync(Company company)
        {
            if (company == null)
                throw new ValidationException("Company data is required.");

            // Igonore Id from client, generate new one | double validation
            company.Id = Guid.NewGuid();

            // Dynamic validation using IRuleManager - ValidationException = 400 Bad Request
            var validationResult = _ruleManager.Validate(company);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            return await _companyRepository.CreateAsync(company);
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _companyRepository.GetAllAsync();
        }

        public async Task<Company> GetByIdAsync(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                throw new NotFoundException($"Company with ID {id} not found.");
            
            return company;
        }

        public async Task<Company> UpdateAsync(Company company)
        {
            if (company == null)
                throw new ValidationException("Company data is required.");

            // Verify if company exists before validations
            var existing = await _companyRepository.GetByIdAsync(company.Id);
            if (existing == null)
                throw new NotFoundException($"Company with ID {company.Id} not found.");

            // Dynamic validation using IRuleManager on Update too
            var validationResult = _ruleManager.Validate(company);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }
                
            return await _companyRepository.UpdateAsync(company);
        }
    }
}
