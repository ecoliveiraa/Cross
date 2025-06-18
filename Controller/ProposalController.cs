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
    public class ProposalController : ControllerBase
    {
        private readonly IProposalService _proposalService;
        private readonly ILeadService _leadService;

        public ProposalController(IProposalService proposalService, ILeadService leadService)
        {
            _proposalService = proposalService;
            _leadService = leadService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProposalCreateDto dto)
        {
            if (dto == null) 
            return BadRequest("Proposal data is required.");
            var lead = await _leadService.GetLeadByIdAsync(dto.LeadID);
            if( lead == null) return NotFound("Lead not found.");


            var proposal = new Proposal
            {
                LeadID = dto.LeadID,
                ProductIDs = dto.ProductIDs,
                ProductionCost = dto.ProductionCost,
                MonthlyProducedProducts = dto.MonthlyProducedProducts,
                ExpectedMonthlyProfit = dto.ExpectedMonthlyProfit,
                CompanyId = lead.CompanyId,
                Country = lead.Country,
                BusinessType = lead.BusinessType
            };
            if( proposal == null) return NotFound("Lead not found.");

            

            

            var created = await _proposalService.CreateAsync(proposal);

            // Update the lead status to "Finalized" after creating the proposal   
            //TODO: Ver se esta bem
            if(created == null) return NotFound("Proposal not created.");
            lead.Status="Finalized";
            await _leadService.UpdateLeadAsync(lead);

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

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProposalUpdateDto dto)
        {
            var existing = await _proposalService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (dto.LeadID.HasValue)
            {
                existing.LeadID = dto.LeadID.Value;
            }
            if (dto.ProductIDs != null)
            {
                existing.ProductIDs = dto.ProductIDs;

            }
            if (dto.ProductionCost != null)
            {
                existing.ProductionCost = dto.ProductionCost.Value;
            }
            if (dto.MonthlyProducedProducts != null)
            {
                existing.MonthlyProducedProducts = dto.MonthlyProducedProducts.Value;
            }
            if (dto.ExpectedMonthlyProfit != null)
            {
                existing.ExpectedMonthlyProfit = dto.ExpectedMonthlyProfit.Value;
            }
            if (dto.Status != null)
            {
                existing.Status = dto.Status;
            }

            var updated = await _proposalService.UpdateAsync(existing);
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