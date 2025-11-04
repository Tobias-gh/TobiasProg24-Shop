using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<CartItem>> GetByCartIdAsync(Guid cartId);
        Task<CartItem?> GetByCartAndProductAsync(Guid cartId, Guid productId);
        Task<CartItem> CreateAsync(CartItem cartItem);
        Task<CartItem> UpdateAsync(CartItem cartItem);
        Task<bool> DeleteAsync(Guid id);
        Task<int> DeleteByCartIdAsync(Guid cartId);

    }
}
