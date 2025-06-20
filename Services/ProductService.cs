using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using WebApplication1.Validation;

namespace WebApplication1.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IRuleManager _ruleManager;

        public ProductService(IProductRepository productRepository, IRuleManager ruleManager)
        {
            _productRepository = productRepository;
            _ruleManager = ruleManager;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            if (product == null)
                throw new ValidationException("Product data is required.");

            // Ignore ProductID from client, generate new one | double validation
            product.ProductID = Guid.NewGuid();

            // Business validation: check if DependentProductId is already set
            if (product.DependentProductId.HasValue)
            {
                // GetByIdAsync wil throw NotFoundException if the product does not exist
                var dependentProduct = await _productRepository.GetByIdAsync(product.DependentProductId.Value);
                if (dependentProduct == null)
                    throw new ValidationException("Referenced dependent product does not exist.");
                
                // Verify if ProductType wasn't already set
                if (dependentProduct.ProductType != product.ProductType)
                    throw new ValidationException("Product and its dependency must have the same ProductType.");
            }

            // Dinamic validation using IRuleManager - ValidationException = 400 Bad Request
            var validationResult = _ruleManager.Validate(product);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            return await _productRepository.CreateAsync(product);
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with ID {id} not found.");
            
            return product;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            if (product == null)
                throw new ValidationException("Product data is required.");

            // Verify if product exists before validations
            var existing = await _productRepository.GetByIdAsync(product.ProductID);
            if (existing == null)
                throw new NotFoundException($"Product with ID {product.ProductID} not found.");

            // Verify if DependentProductId on update too
            if (product.DependentProductId.HasValue)
            {
                var dependentProduct = await _productRepository.GetByIdAsync(product.DependentProductId.Value);
                if (dependentProduct == null)
                    throw new ValidationException("Referenced dependent product does not exist.");
                
                
                if (dependentProduct.ProductType != product.ProductType)
                    throw new ValidationException("Product and its dependency must have the same ProductType.");
            }

            // Dynamic validation using IRuleManager on Update too
            var validationResult = _ruleManager.Validate(product);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            return await _productRepository.UpdateAsync(product);
        }
    }
}