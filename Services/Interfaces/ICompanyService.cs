using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Services.Interfaces
{
  public interface ICompanyService
  {
    Task<Company> CreateAsync(Company company);
    Task<List<Company>> GetAllAsync();
    Task<Company> GetByIdAsync(Guid id);
    Task UpdateAsync(Company company);
  }
}