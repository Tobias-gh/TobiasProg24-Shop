using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{

    private readonly ShopDbContext _context;


    public CartRepository(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetBySessionIdAsync(string session)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(c => c.SessionId == session);

    }
    public async Task<Cart?> GetByIdAsync(Guid id)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(c => c.Id == id);
    }


    public async Task<Cart> CreateAsync(Cart cart)
    {
        cart.CreatedAt = DateTime.UtcNow;
        cart.UpdatedAt = DateTime.UtcNow;

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(cart.Id))!;


    }
    public async Task<Cart> UpdateAsync(Cart cart)
    {
       cart.UpdatedAt = DateTime.UtcNow;

        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(cart.Id))!;
    }

    public async Task<bool> DeleteAscyn(Guid id)
    {
        var cart = await _context.Carts.FindAsync(id);
        if(cart == null) return false;

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBySessionAsync(string session)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.SessionId == session);
        if (cart == null) return false;

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();
        return true;

    }

 
}


