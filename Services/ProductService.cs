using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            // Validações de negócio se necessário
            if (product.ProductID == Guid.Empty)
                product.ProductID = Guid.NewGuid();

            // Se tem DependentProductId, validar se esse produto existe
            if (product.DependentProductId.HasValue)
            {
                var dependentProduct = await _productRepository.GetByIdAsync(product.DependentProductId.Value);
                if (dependentProduct == null)
                    return null; // Ou lançar exception - produto dependente não existe
            }

            return await _productRepository.CreateAsync(product);
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            // Validação se existe
            var existing = await _productRepository.GetByIdAsync(product.ProductID);
            if (existing == null)
                return null;

            // Validar DependentProductId se foi alterado
            if (product.DependentProductId.HasValue)
            {
                var dependentProduct = await _productRepository.GetByIdAsync(product.DependentProductId.Value);
                if (dependentProduct == null)
                    return null; // Produto dependente não existe
            }

            return await _productRepository.UpdateAsync(product);
        }
    }
}
