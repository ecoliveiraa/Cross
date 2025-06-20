using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Repository
{
    public class InMemoryProposalRepository : IProposalRepository
    {
        private readonly List<Proposal> _proposals = new();

        public Task<Proposal> CreateAsync(Proposal proposal)
        {
            _proposals.Add(proposal);
            return Task.FromResult(proposal);
        }

        public Task<Proposal> GetByIdAsync(Guid id) =>
            Task.FromResult(_proposals.FirstOrDefault(p => p.ProposalID == id));

        public Task<List<Proposal>> GetAllAsync() => 
            Task.FromResult(_proposals);

        public Task<Proposal> UpdateAsync(Proposal proposal)
        {
            var existing = _proposals.FirstOrDefault(p => p.ProposalID == proposal.ProposalID);
            
            _proposals[_proposals.IndexOf(existing)] = proposal; 
            
            return Task.FromResult(proposal);
        }
    }
}