using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.InMemory
{
    public class InMemoryProductService : IProductService
    {
        private readonly List<Product> _products = new();

        public Task<Product> CreateAsync(Product product)
        {
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task<Product> GetByIdAsync(Guid id) =>
            Task.FromResult(_products.FirstOrDefault(p => p.ProductID == id));

        public Task<List<Product>> GetAllAsync() => Task.FromResult(_products);

        public Task<Product> UpdateAsync(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.ProductID == product.ProductID);
            if (existing != null)
            {
                existing.DependentProductId = product.DependentProductId;
                existing.ProductType = product.ProductType;
            }
            return Task.FromResult(existing);
        }
    }
}