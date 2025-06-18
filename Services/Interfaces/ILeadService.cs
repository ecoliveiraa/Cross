using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface ILeadService
    {
        Task<Lead> CreateLeadAsync(Lead lead);
        Task<List<Lead>> GetAllLeadsAsync();
        Task<Lead> GetLeadByIdAsync(Guid leadId);
        Task<Lead> UpdateLeadAsync(Lead updatedLead);
    }
}