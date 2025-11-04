using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ShopDbContext _context;

        public CartItemRepository(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem> CreateAsync(CartItem cartItem)
        {

            cartItem.AddedAt = DateTime.UtcNow;
            _context.CartItems.Add(cartItem);
            await  _context.SaveChangesAsync();

            return (await GetByIdAsync(cartItem.Id))!;

        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if(cartItem == null) return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> DeleteByCartIdAsync(Guid cartId)
        {
            var items = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return items.Count;

        }

        public async Task<CartItem?> GetByCartAndProductAsync(Guid cartId, Guid productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task<IEnumerable<CartItem>> GetByCartIdAsync(Guid cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.Category)
                .Include(ci => ci.Cart)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }

        public async Task<CartItem?> GetByIdAsync(Guid id)
        {
            return await _context.CartItems
               .Include(ci => ci.Product)
                   .ThenInclude(p => p.Category)
               .Include(ci => ci.Cart)
               .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<CartItem> UpdateAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(cartItem.Id))!;
        }
    }
}
