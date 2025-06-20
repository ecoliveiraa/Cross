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
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Creates a new company
        /// </summary>
        /// <param name="company">Company data</param>
        /// <returns>Created company</returns>
        /// <remarks>
        /// **Dynamic validation rules:**
        /// - NIF is required when Country = "Portugal"
        /// </remarks>
        /// <response code="201">Company created successfully</response>
        /// <response code="400">Validation failed - check required fields and dynamic rules</response>
        [HttpPost]
        [ProducesResponseType(typeof(Company), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Create([FromBody] Company company)
        {
            try
            {
                var created = await _companyService.CreateAsync(company);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets all companies
        /// </summary>
        /// <returns>List of all companies</returns>
        /// <response code="200">Returns the list of companies</response>
        [HttpGet]
        [ProducesResponseType(typeof(Company[]), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var companies = await _companyService.GetAllAsync();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a specific company by ID
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <returns>Company details</returns>
        /// <response code="200">Returns the company</response>
        /// <response code="404">Company not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Company), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var company = await _companyService.GetByIdAsync(id);
                return Ok(company);
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
        /// Updates a company (complete replacement)
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <param name="company">Updated company data</param>
        /// <returns>Updated company</returns>
        /// <remarks>
        /// **Dynamic validation rules:**
        /// - NIF is required when Country = "Portugal"
        /// 
        /// **Note:** This is a complete replacement (PUT). All fields must be provided.
        /// </remarks>
        /// <response code="200">Company updated successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="404">Company not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Company), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] Company company)
        {
            try
            {
                company.Id = id;
                var updated = await _companyService.UpdateAsync(company);
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