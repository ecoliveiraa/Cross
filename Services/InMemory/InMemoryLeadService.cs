using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.InMemory
{
  public class InMemoryLeadService : ILeadService
  {
    private readonly List<Lead> _leads = new(); // Fake in-memory storage  readonly to make the list immutable not  the object itself

    public Task<Lead> CreateLeadAsync(Lead lead)
    {
        // lead.Id = Guid.NewGuid() ||| not needed because  the Id is already set in the constructor of the Lead class
        _leads.Add(lead);
        return Task.FromResult(lead);
    }

    public Task<List<Lead>> GetAllLeadsAsync()
    {
        return Task.FromResult(_leads);
    }

    public Task<Lead> GetLeadByIdAsync(Guid leadId)
    {
        var lead = _leads.FirstOrDefault(l => l.LeadID == leadId);
        return Task.FromResult(lead);
    }

    public Task<Lead> UpdateLeadAsync(Lead updatedLead)
    {
      var existingLead = _leads.FirstOrDefault(l => l.LeadID == updatedLead.LeadID);
      if (existingLead != null)
      {
          // just update the properties that are not null
          if (updatedLead.CompanyId != null)
              existingLead.CompanyId = updatedLead.CompanyId;;

          if (updatedLead.Country != null)
              existingLead.Country = updatedLead.Country;

          if (updatedLead.BusinessType != null)
              existingLead.BusinessType = updatedLead.BusinessType;

          if (updatedLead.Status != null)
              existingLead.Status = updatedLead.Status;
      }

      return Task.FromResult(existingLead);
    }

  }
}