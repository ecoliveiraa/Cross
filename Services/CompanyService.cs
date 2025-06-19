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
            //Dynamic validation using IRuleManager
            var validationResult = _ruleManager.Validate(company);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            if (company.Id == Guid.Empty)
                company.Id = Guid.NewGuid();

            return await _companyRepository.CreateAsync(company);
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _companyRepository.GetAllAsync();
        }

        public async Task<Company> GetByIdAsync(Guid id)
        {
            return await _companyRepository.GetByIdAsync(id);
        }

        public async Task<Company> UpdateAsync(Company company)
        {
            //Dynamic validation using IRuleManager on Update too
            var validationResult = _ruleManager.Validate(company);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            var existing = await _companyRepository.GetByIdAsync(company.Id);
            if (existing == null)
                return null;
                
            return await _companyRepository.UpdateAsync(company);
        }
    }
}