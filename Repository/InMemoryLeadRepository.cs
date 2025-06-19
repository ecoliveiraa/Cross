using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Repository
{
    public class InMemoryLeadRepository : ILeadRepository
    {
        private readonly List<Lead> _leads = new();

        public Task<Lead> CreateAsync(Lead lead)
        {
            _leads.Add(lead);
            return Task.FromResult(lead);
        }

        public Task<List<Lead>> GetAllAsync()
        {
            return Task.FromResult(_leads);
        }

        public Task<Lead> GetByIdAsync(Guid id)
        {
            var lead = _leads.FirstOrDefault(l => l.LeadID == id);
            return Task.FromResult(lead);
        }

        public Task<Lead> UpdateAsync(Lead lead)
        {
            var existing = _leads.FirstOrDefault(l => l.LeadID == lead.LeadID);
            if (existing != null)
            {
                _leads[_leads.IndexOf(existing)] = lead; // Substituição completa
            }
            return Task.FromResult(lead);
        }
    }
}
