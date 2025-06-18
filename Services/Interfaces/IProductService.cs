using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IProductService
    {
      Task<Product> CreateAsync(Product product);
      Task<Product> GetByIdAsync(Guid id);
      Task<List<Product>> GetAllAsync();
      Task<Product> UpdateAsync(Product product);
    }
}