using Microsoft.AspNetCore.Mvc;
using Shop.Application.Dtos;
using Shop.Application.Services.Cart;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{sessionId}")]
    public async Task<ActionResult<CartDto>> GetCart(string sessionId)
    {
        var cart = await _cartService.GetOrCreateCartAsync(sessionId);
        return Ok(cart);
    }

    [HttpGet("{sessionId}/summary")]
    public async Task<ActionResult<CartSummaryDto>> GetCartSummary(string sessionId)
    {
        var summary = await _cartService.GetCartSummaryAsync(sessionId);
        return Ok(summary);
    }

   
    [HttpPost("{sessionId}/items")]
    public async Task<ActionResult<CartDto>> AddItem(string sessionId, [FromBody] AddToCartRequest request)
    {
        try
        {
            var cart = await _cartService.AddItemToCartAsync(sessionId, request);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{sessionId}/items/{cartItemId}")]
    public async Task<ActionResult<CartDto>> UpdateItemQuantity(string sessionId, Guid cartItemId, [FromBody] UpdateCartItemRequest request)
    {
        try
        {
            var cart = await _cartService.UpdateCartItemQuantityAsync(sessionId, cartItemId, request.Quantity);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("{sessionId}/items/{cartItemId}")]
    public async Task<ActionResult<CartDto>> RemoveItem(string sessionId, Guid cartItemId)
    {
        try
        {
            var cart = await _cartService.RemoveItemFromCartAsync(sessionId, cartItemId);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("{sessionId}")]
    public async Task<ActionResult> ClearCart(string sessionId)
    {
        try
        {
            await _cartService.ClearCartAsync(sessionId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
