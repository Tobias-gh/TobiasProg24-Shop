using Shop.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services.Cart;

public interface ICartService
{
    Task<CartDto> GetOrCreateCartAsync(string sessionId);
    Task<CartDto?> GetCartBySessionIdAsync(string sessionId);
    Task<CartDto> AddItemToCartAsync(string sessionId, AddToCartRequest request);
    Task<CartDto> UpdateCartItemQuantityAsync(string sessionId, Guid CartItemId, int quantity);
    Task<CartDto> RemoveItemFromCartAsync(string sessionId, Guid CartItemId);
    Task<bool> ClearCartAsync(string sessionId);
    Task<CartSummaryDto> GetCartSummaryAsync(string sessionId);
}
