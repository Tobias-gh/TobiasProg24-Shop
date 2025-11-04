using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ShopDbContext _context;
    public ProductRepository(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid productId)
    {
        return await _context.Products
          .Include(p => p.Category)
          .FirstOrDefaultAsync(p => p.Id == productId);
    }


}

