using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.InMemory
{
    public class InMemoryCompanyService : ICompanyService
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
            if (existing != null)
            {
                existing.NIF = company.NIF;
                existing.Address = company.Address;
                existing.Country = company.Country;
                existing.Status = company.Status;
                existing.Stakeholder = company.Stakeholder;
                existing.Contact = company.Contact;
            }
            return Task.FromResult(company);
        }
    }
}