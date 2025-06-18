using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadController : ControllerBase
    {
      private readonly ILeadService _leadService;
      private readonly ICompanyService _companyService;

      public LeadController(ILeadService leadService , ICompanyService companyService)
      {
          _leadService = leadService;
          _companyService = companyService;
      }

      [HttpPost]
      public async Task<IActionResult> Create([FromBody] LeadCreateDto dto)
      {
          if (dto == null) 
              return BadRequest("Lead data is required.");
          var company = await _companyService.GetByIdAsync(dto.CompanyId);
          if (company == null) 
              return NotFound("Company not found.");
          var lead = new Lead
          {
              CompanyId = dto.CompanyId,
              Country = company.Country,
              BusinessType = dto.BusinessType,
              
          };
          var created = await _leadService.CreateLeadAsync(lead);
          return CreatedAtAction(nameof(GetById), new { id = lead.LeadID }, lead);
      }

      [HttpGet("{id}")]
      public async Task<IActionResult> GetById(Guid id)
      {
          var lead = await _leadService.GetLeadByIdAsync(id);
          if (lead == null) return NotFound();
          return Ok(lead);
      }
      [HttpGet]
      public async Task<IActionResult> GetAll()
      {
          var leads = await _leadService.GetAllLeadsAsync();
          return Ok(leads);
      }

      [HttpPatch("{id}")]
      public async Task<IActionResult> Update(Guid id, [FromBody] LeadUpdateDto dto)
      {
        var existing = await _leadService.GetLeadByIdAsync(id);
        if (existing == null)
            return NotFound();

        if (dto.CompanyId.HasValue)
            existing.CompanyId = dto.CompanyId.Value;
        if (!string.IsNullOrWhiteSpace(dto.Country))
            existing.Country = dto.Country;
        if (!string.IsNullOrWhiteSpace(dto.BusinessType))
            existing.BusinessType = dto.BusinessType;
        if (!string.IsNullOrWhiteSpace(dto.Status))
            existing.Status = dto.Status;

        var updated = await _leadService.UpdateLeadAsync(existing);
        return Ok(updated);
      }
      
    }
}