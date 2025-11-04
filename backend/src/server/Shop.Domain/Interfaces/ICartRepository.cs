using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetBySessionIdAsync(string session);
        Task<Cart?> GetByIdAsync(Guid id);
        Task<Cart> CreateAsync(Cart cart);
        Task<Cart> UpdateAsync(Cart cart);
        Task<bool> DeleteAscyn(Guid id);
        Task<bool> DeleteBySessionAsync(string session);
    }
}
