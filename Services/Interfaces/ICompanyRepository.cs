using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> CreateAsync(Company company);
        Task<List<Company>> GetAllAsync();
        Task<Company> GetByIdAsync(Guid id);
        Task<Company> UpdateAsync(Company company);
    }
}