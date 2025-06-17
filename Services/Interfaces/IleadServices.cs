using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Services.Interfaces
{
    public interface IleadServices
    {
        Tasks<Lead> CreateLeadAsync(Lead lead);
        Task<List<Lead>> GetAllLeadsAsync();
        Task<Lead> GetLeadByIdAsync(Guid leadId);
        Task<Lead> UpdateLeadAsync(Lead updatedLead);
    }
}