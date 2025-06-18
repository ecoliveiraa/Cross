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
     public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Company company)
        {
            var created = await _companyService.CreateAsync(company);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null) return NotFound();
            return Ok(company);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CompanyUpdateDto dto)
        {
            var existing = await _companyService.GetByIdAsync(id);
            if (existing == null) return NotFound();

           if (dto.Country ! = null)
            {  
                existing.Country = dto.Country;
            }
            if (dto.NIF != null)
            {
                existing.NIF = dto.NIF;
            }
            if (dto.Address != null)
            {
                existing.Address = dto.Address;
            }
            if (dto.Status != null)
            {
                existing.Status = dto.Status;
            }
            if (dto.Stakeholder != null)
            {
                existing.Stakeholder = dto.Stakeholder;
            }
            if (dto.Contact != null)
            {
                existing.Contact = dto.Contact;
            }
            var updated = await _companyService.UpdateAsync(existing);
            return Ok(updated);
        }
    }
}