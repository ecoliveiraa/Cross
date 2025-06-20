using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Repository
{
    public class InMemoryCompanyRepository : ICompanyRepository
    {
        private readonly List<Company> _companies = new();

        public Task<Company> CreateAsync(Company company)
        {
            _companies.Add(company);
            return Task.FromResult(company);
        }

        public Task<List<Company>> GetAllAsync() => Task.FromResult(_companies);

        public Task<Company> GetByIdAsync(Guid id) =>
            Task.FromResult(_companies.FirstOrDefault(c => c.Id == id));

        public Task<Company> UpdateAsync(Company company) 
        {
            var existing = _companies.FirstOrDefault(c => c.Id == company.Id);
            
            
              _companies[_companies.IndexOf(existing)] = company;
            
            return Task.FromResult(company); 
        }
    }
}