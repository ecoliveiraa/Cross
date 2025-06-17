using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Services.Interfaces
{
  public interface IProposalService
  {
    Task<Proposal> CreateAsync(Proposal proposal);
    Task<Proposal> GetByIdAsync(Guid id);
    Task<List<Proposal>> GetAllAsync();
    Task AddProductAsync(Guid proposalId, Guid productId);
    Task FinalizeProposalAsync(Guid proposalId);
    Task UpdateAsync(Proposal proposal);
  }
}