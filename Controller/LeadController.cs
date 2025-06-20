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

        /// <summary>
        /// Creates a new lead from a company
        /// </summary>
        /// <param name="lead">Lead data</param>
        /// <returns>Created lead</returns>
        /// <remarks>
        /// **Dynamic validation rules:**
        /// - BusinessType is required when Status = "Active"
        /// 
        /// **Auto-populated fields:**
        /// - Country is inherited from the associated company
        /// </remarks>
        /// <response code="201">Lead created successfully</response>
        /// <response code="400">Validation failed or invalid CompanyId</response>
        /// <response code="404">Company not found</response>
        [HttpPost]
        [ProducesResponseType(typeof(Lead), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Create([FromBody] Lead lead)
        {
            try
            {
                var created = await _leadService.CreateAsync(lead);
                return CreatedAtAction(nameof(GetById), new { id = created.LeadID }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a specific lead by ID
        /// </summary>
        /// <param name="id">Lead ID</param>
        /// <returns>Lead details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Lead), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var lead = await _leadService.GetByIdAsync(id);
                return Ok(lead);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets all leads
        /// </summary>
        /// <returns>List of all leads</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Lead[]), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var leads = await _leadService.GetAllAsync();
                return Ok(leads);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a lead (complete replacement)
        /// </summary>
        /// <param name="id">Lead ID</param>
        /// <param name="lead">Updated lead data</param>
        /// <returns>Updated lead</returns>
        /// <remarks> 
        /// **Dynamic validation rules:**
        /// - BusinessType is required when Status = "Active"
        /// </remarks>
        /// <response code="200">Lead updated successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="404">Lead not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Lead), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] Lead lead)
        {
            try
            {
                lead.LeadID = id;
                var updated = await _leadService.UpdateAsync(lead);
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}