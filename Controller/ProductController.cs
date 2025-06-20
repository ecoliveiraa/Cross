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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="product">Product data</param>
        /// <returns>Created product</returns>
        /// <remarks>
        /// **Business rules:**
        /// - If DependentProductId is provided, the referenced product must exist
        /// - If DependentProductId is provided, both products must have the same ProductType
        /// - Products with dependencies cannot be created before their required product exists
        /// </remarks>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">Validation failed - invalid dependent product reference</response>
        [HttpPost]
        [ProducesResponseType(typeof(Product), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            try
            {
                var created = await _productService.CreateAsync(product);
                return CreatedAtAction(nameof(GetById), new { id = created.ProductID }, created);
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
        /// Gets all products
        /// </summary>
        /// <returns>List of all products</returns>
        /// <remarks>
        /// Returns all products including both independent products and products with dependencies.
        /// Use this endpoint to see the complete product catalog.
        /// </remarks>
        /// <response code="200">Returns the list of products</response>
        [HttpGet]
        [ProducesResponseType(typeof(Product[]), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a specific product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        /// <remarks>
        /// Retrieves detailed information about a specific product, including its dependencies if any.
        /// </remarks>
        /// <response code="200">Returns the product</response>
        /// <response code="404">Product not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(product);
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
        /// Updates a product (complete replacement)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="product">Updated product data</param>
        /// <returns>Updated product</returns>
        /// <remarks>
        /// **Business rules:**
        /// - If changing DependentProductId, the new referenced product must exist
        /// - If setting DependentProductId, both products must have the same ProductType
        /// - Cannot create circular dependencies
        /// 
        /// **Note:** This is a complete replacement (PUT). All fields must be provided.
        /// </remarks>
        /// <response code="200">Product updated successfully</response>
        /// <response code="400">Validation failed - invalid dependency or circular reference</response>
        /// <response code="404">Product not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product product)
        {
            try
            {
                product.ProductID = id;
                var updated = await _productService.UpdateAsync(product);
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