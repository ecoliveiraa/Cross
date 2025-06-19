using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<Company> CreateAsync(Company company)
        {
            // Validações de negócio se necessário
            if (company.Id == Guid.Empty)
            {
                company.Id = Guid.NewGuid();
            }

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
            // Validação se a company existe
            var existing = await _companyRepository.GetByIdAsync(company.Id);
            if (existing == null)
                return null;
                
            return await _companyRepository.UpdateAsync(company);
        }
    }
}