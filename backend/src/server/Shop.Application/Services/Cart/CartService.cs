using Microsoft.VisualBasic;
using Shop.Application.Dtos;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DomainCart = Shop.Domain.Entities.Cart;
using DomainCartItem = Shop.Domain.Entities.CartItem;

namespace Shop.Application.Services.Cart;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetOrCreateCartAsync(string sessionId)
    {
        var cart = await _cartRepository.GetBySessionIdAsync(sessionId);

        if(cart == null)
        {
            cart = new DomainCart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId

            };
            cart = await _cartRepository.CreateAsync(cart);
        }

        return MapToDto(cart);


    }
    public async Task<CartDto?> GetCartBySessionIdAsync(string sessionId)
    {
        var cart = await _cartRepository.GetBySessionIdAsync(sessionId);
        return cart == null ? null :  MapToDto(cart);
    }

    public async Task<CartDto> AddItemToCartAsync(string sessionId, AddToCartRequest request)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with id {request.ProductId} not found.");
        }

        if (request.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero");
        }

        if (product.Stock < request.Quantity)
        {
            throw new InvalidOperationException($"Insufficient stock. Only {product.Stock} items available");
        }

        var cart = await _cartRepository.GetBySessionIdAsync(sessionId);
        if (cart == null)
        {
            cart = new DomainCart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId
            };
            cart = await _cartRepository.CreateAsync(cart);
        }

        //check if product alreadt in cart
        var existingItem = await _cartItemRepository.GetByCartAndProductAsync(cart.Id, request.ProductId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + request.Quantity;


            if (newQuantity > product.Stock)
            {
                throw new InvalidOperationException(
                    $"Cannot add {request.Quantity} more. Total would be {newQuantity}, but only {product.Stock} available.");

            }
            existingItem.Quantity = newQuantity;
            await _cartItemRepository.UpdateAsync(existingItem);
        }
        else
        {
            var cartItem = new DomainCartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
            await _cartItemRepository.CreateAsync(cartItem);
        }

        await _cartRepository.UpdateAsync(cart);
        cart = await _cartRepository.GetBySessionIdAsync(sessionId);
        return MapToDto(cart!);

    }

    public async Task<bool> ClearCartAsync(string sessionId)
    {
        var cart = await _cartRepository.GetBySessionIdAsync(sessionId);
        if (cart == null) return false;

        await _cartItemRepository.DeleteByCartIdAsync(cart.Id);

        await _cartRepository.UpdateAsync(cart);

        return true;

    }

    public async Task<CartSummaryDto> GetCartSummaryAsync(string sessionId)
    {
        var cart = await _cartRepository.GetBySessionIdAsync(sessionId);
        if(cart == null || !cart.Items.Any())
        {
            return new CartSummaryDto(0, 0m);
        }

        var totalItems = cart.Items.Sum(i => i.Quantity);
        var totalPrice = cart.Items.Sum(i => i.Quantity * i.Product.Price);

        return new CartSummaryDto(totalItems, totalPrice);
    }

    public async Task<CartDto> RemoveItemFromCartAsync(string sessionId, Guid CartItemId)
    {
        var cart = await _cartRepository.GetBySessionIdAsync(sessionId);
     if (cart == null)
     {
      throw new KeyNotFoundException("Cart not found.");
        }

        var cartItem = await _cartItemRepository.GetByIdAsync(CartItemId);
        if(cartItem ==null || cartItem.CartId != cart.Id)
        {
 throw new KeyNotFoundException("Cart item not found.");
        }

        await _cartItemRepository.DeleteAsync(CartItemId);
      await _cartRepository.UpdateAsync(cart);

        cart = await _cartRepository.GetBySessionIdAsync(sessionId);
   return MapToDto(cart!);
    }

    public async Task<CartDto> UpdateCartItemQuantityAsync(string sessionId, Guid CartItemId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than 0.");
        }

        var cart = await _cartRepository.GetBySessionIdAsync(sessionId);
        if(cart == null)
        {
    throw new KeyNotFoundException("Cart not found.");
 }

        var cartItem = await _cartItemRepository.GetByIdAsync(CartItemId);
        if(cartItem == null || cartItem.CartId != cart.Id)
        {
    throw new KeyNotFoundException("Cart item not found.");
        }

        //validate stock
  var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
        if(product == null)
        {
    throw new KeyNotFoundException("Product not found.");
        }
if (quantity > product.Stock)
        {
          throw new InvalidOperationException(
        $"Insufficient stock. Only {product.Stock} items available.");
        }

        cartItem.Quantity = quantity;
        await _cartItemRepository.UpdateAsync(cartItem);

        await _cartRepository.UpdateAsync(cart);

        //reload cart
        cart = await _cartRepository.GetBySessionIdAsync(sessionId);
  return MapToDto(cart!);
    }



    private static CartDto MapToDto(DomainCart cart)
    {
        var items = cart.Items.Select(ci => new CartItemDto(
            ci.Id,
            ci.ProductId,
            ci.Product.Name,
            ci.Product.Description,
            ci.Product.Price,
            ci.Quantity,
            ci.Quantity * ci.Product.Price,
            ci.Product.Stock,
            ci.AddedAt
        )).ToList();

        var totalItems = items.Sum(i => i.Quantity);
        var totalPrice = items.Sum(i => i.Subtotal);

        return new CartDto(
            cart.Id,
            cart.SessionId,
            items,
            totalItems,
            totalPrice,
            cart.CreatedAt,
            cart.UpdatedAt
        );
    }
}
