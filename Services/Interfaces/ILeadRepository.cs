using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface ILeadRepository
    {
        Task<Lead> CreateAsync(Lead lead);
        Task<List<Lead>> GetAllAsync();
        Task<Lead> GetByIdAsync(Guid id);
        Task<Lead> UpdateAsync(Lead lead);
    }
}
