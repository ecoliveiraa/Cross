using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Services.Interfaces
{
    public interface IProductService
    {
      Task<Product> CreateAsync(Product product);
      Task<Product> GetByIdAsync(Guid id);
      Task<List<Product>> GetAllAsync();
      Task UpdateAsync(Product product);
    }
}