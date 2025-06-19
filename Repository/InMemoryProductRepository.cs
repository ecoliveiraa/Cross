using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Repository
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public Task<Product> CreateAsync(Product product)
        {
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task<Product> GetByIdAsync(Guid id) =>
            Task.FromResult(_products.FirstOrDefault(p => p.ProductID == id));

        public Task<List<Product>> GetAllAsync() => 
            Task.FromResult(_products);

        public Task<Product> UpdateAsync(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.ProductID == product.ProductID);
            if (existing != null)
            {
                _products[_products.IndexOf(existing)] = product; // Substituição completa
            }
            return Task.FromResult(product);
        }
    }
}