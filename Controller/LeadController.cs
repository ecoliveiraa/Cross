using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using WebApplication1.Validation;

namespace WebApplication1.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadController : ControllerBase
    {
        private readonly ILeadService _leadService;

        public LeadController(ILeadService leadService)
        {
            _leadService = leadService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Lead lead)
        {
            try
            {
                if (lead == null)
                    return BadRequest("Lead data is required.");

                var created = await _leadService.CreateAsync(lead);
                if (created == null)
                    return NotFound("Company not found.");

                return CreatedAtAction(nameof(GetById), new { id = created.LeadID }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var lead = await _leadService.GetByIdAsync(id);
            if (lead == null) return NotFound();
            return Ok(lead);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leads = await _leadService.GetAllAsync();
            return Ok(leads);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Lead lead)
        {
            try
            {
                lead.LeadID = id;

                var updated = await _leadService.UpdateAsync(lead);
                if (updated == null) return NotFound();

                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}