using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IProposalRepository
    {
        Task<Proposal> CreateAsync(Proposal proposal);
        Task<Proposal> GetByIdAsync(Guid id);
        Task<List<Proposal>> GetAllAsync();
        Task<Proposal> UpdateAsync(Proposal proposal);
    }
}