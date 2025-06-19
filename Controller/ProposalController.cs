using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Proposal proposal)
        {
            if (proposal == null) 
                return BadRequest("Proposal data is required.");

            var created = await _proposalService.CreateAsync(proposal);
            if (created == null) 
                return NotFound("Lead not found.");

            return CreatedAtAction(nameof(GetById), new { id = created.ProposalID }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var proposals = await _proposalService.GetAllAsync();
            return Ok(proposals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var proposal = await _proposalService.GetByIdAsync(id);
            if (proposal == null) return NotFound();
            return Ok(proposal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Proposal proposal)
        {
            proposal.ProposalID = id;

            var updated = await _proposalService.UpdateAsync(proposal);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpPost("{proposalId}/add-product/{productId}")]
        public async Task<IActionResult> AddProduct(Guid proposalId, Guid productId)
        {
            try
            {
                await _proposalService.AddProductAsync(proposalId, productId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{proposalId}/finalize")]
        public async Task<IActionResult> Finalize(Guid proposalId)
        {
            try
            {
                await _proposalService.FinalizeProposalAsync(proposalId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}