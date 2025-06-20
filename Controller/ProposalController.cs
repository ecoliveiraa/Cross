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
    public class ProposalController : ControllerBase
    {
        private readonly IProposalService _proposalService;

        public ProposalController(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        /// <summary>
        /// Creates a new proposal from a lead
        /// </summary>
        /// <param name="proposal">Proposal data</param>
        /// <returns>Created proposal</returns>
        /// <remarks>
        /// **Dynamic validation rules:**
        /// - ProductionCost is required when Status != "Draft"
        /// - MonthlyProducedProducts is required when Status = "Finalized"
        /// - ProductIDs cannot be empty when Status = "Finalized"
        /// 
        /// **Business rules:**
        /// - When proposal is created, the associated lead status changes to "Finalized"
        /// </remarks>
        /// <response code="201">Proposal created successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="404">Lead not found</response>
        [HttpPost]
        [ProducesResponseType(typeof(Proposal), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Create([FromBody] Proposal proposal)
        {
            try
            {
                var created = await _proposalService.CreateAsync(proposal);
                return CreatedAtAction(nameof(GetById), new { id = created.ProposalID }, created);
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
        /// Gets all proposals
        /// </summary>
        /// <returns>List of all proposals</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Proposal[]), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var proposals = await _proposalService.GetAllAsync();
                return Ok(proposals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a specific proposal by ID
        /// </summary>
        /// <param name="id">Proposal ID</param>
        /// <returns>Proposal details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Proposal), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var proposal = await _proposalService.GetByIdAsync(id);
                return Ok(proposal);
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
        /// Updates a proposal (complete replacement)
        /// </summary>
        /// <param name="id">Proposal ID</param>
        /// <param name="proposal">Updated proposal data</param>
        /// <returns>Updated proposal</returns>
        /// <remarks>
        /// **Dynamic validation rules:**
        /// - ProductionCost is required when Status != "Draft"
        /// - MonthlyProducedProducts is required when Status = "Finalized"
        /// - ProductIDs cannot be empty when Status = "Finalized"
        /// </remarks>
        /// <response code="200">Proposal updated successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="404">Proposal not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Proposal), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] Proposal proposal)
        {
            try
            {
                proposal.ProposalID = id;
                var updated = await _proposalService.UpdateAsync(proposal);
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

        /// <summary>
        /// Adds a product to a proposal
        /// </summary>
        /// <param name="proposalId">Proposal ID</param>
        /// <param name="productId">Product ID to add</param>
        /// <returns>Success confirmation</returns>
        /// <remarks>
        /// **Business rules:**
        /// - Product must exist
        /// - If product has dependencies, dependent product must be added first
        /// - Products must have same ProductType as dependencies
        /// </remarks>
        /// <response code="200">Product added successfully</response>
        /// <response code="400">Validation failed - missing dependencies or type mismatch</response>
        /// <response code="404">Proposal or Product not found</response>
        [HttpPost("{proposalId}/add-product/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddProduct(Guid proposalId, Guid productId)
        {
            try
            {
                await _proposalService.AddProductAsync(proposalId, productId);
                return Ok();
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
        /// Finalizes a proposal
        /// </summary>
        /// <param name="proposalId">Proposal ID to finalize</param>
        /// <returns>Success confirmation</returns>
        /// <remarks>
        /// **Validation before finalization:**
        /// - ProductionCost must be provided
        /// - MonthlyProducedProducts must be provided  
        /// - ProductIDs cannot be empty
        /// 
        /// **Business rules:**
        /// - When finalized, associated company status changes to "Active"
        /// - All dynamic validation rules are checked
        /// </remarks>
        /// <response code="200">Proposal finalized successfully</response>
        /// <response code="400">Validation failed - missing required fields</response>
        /// <response code="404">Proposal not found</response>
        [HttpPost("{proposalId}/finalize")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Finalize(Guid proposalId)
        {
            try
            {
                await _proposalService.FinalizeProposalAsync(proposalId);
                return Ok();
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