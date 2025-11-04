using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Controllers;
using Shop.Application.Dtos;
using Shop.Application.Services.Cart;

namespace Shop.Api.Tests.Controllers;

public class CartsControllerTests
{
    private readonly ICartService _cartService;
    private readonly CartsController _sut;

    public CartsControllerTests()
    {
        _cartService = A.Fake<ICartService>();
        _sut = new CartsController(_cartService);
    }

    #region GetCart Tests

    [Fact]
    public async Task GetCart_ReturnsOkResult_WithCart()
    {
        // Arrange
        var sessionId = "test-session";
        var cart = new CartDto(
      Guid.NewGuid(),
               sessionId,
       new List<CartItemDto>(),
               0,
     0m,
     DateTime.UtcNow,
          DateTime.UtcNow
      );

        A.CallTo(() => _cartService.GetOrCreateCartAsync(sessionId)).Returns(cart);

        // Act
        var result = await _sut.GetCart(sessionId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(cart);
    }

    [Fact]
    public async Task GetCart_CallsServiceWithCorrectSessionId()
    {
        // Arrange
        var sessionId = "session-123";
        var cart = new CartDto(Guid.NewGuid(), sessionId, new List<CartItemDto>(), 0, 0m, DateTime.UtcNow, DateTime.UtcNow);

        A.CallTo(() => _cartService.GetOrCreateCartAsync(sessionId)).Returns(cart);

        // Act
        await _sut.GetCart(sessionId);

        // Assert
        A.CallTo(() => _cartService.GetOrCreateCartAsync(sessionId)).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region GetCartSummary Tests

    [Fact]
    public async Task GetCartSummary_ReturnsOkResult_WithSummary()
    {
        // Arrange
        var sessionId = "test-session";
        var summary = new CartSummaryDto(5, 150.99m);

        A.CallTo(() => _cartService.GetCartSummaryAsync(sessionId)).Returns(summary);

        // Act
        var result = await _sut.GetCartSummary(sessionId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(summary);
    }

    [Fact]
    public async Task GetCartSummary_CallsServiceOnce()
    {
        // Arrange
        var sessionId = "session-123";
        var summary = new CartSummaryDto(0, 0m);

        A.CallTo(() => _cartService.GetCartSummaryAsync(sessionId)).Returns(summary);

        // Act
        await _sut.GetCartSummary(sessionId);

        // Assert
        A.CallTo(() => _cartService.GetCartSummaryAsync(sessionId)).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region AddItem Tests

    [Fact]
    public async Task AddItem_ReturnsOkResult_WithUpdatedCart()
    {
        // Arrange
        var sessionId = "session-123";
        var request = new AddToCartRequest(Guid.NewGuid(), 2);
        var cart = new CartDto(Guid.NewGuid(), sessionId, new List<CartItemDto>(), 2, 20m, DateTime.UtcNow, DateTime.UtcNow);

        A.CallTo(() => _cartService.AddItemToCartAsync(sessionId, request)).Returns(cart);

        // Act
        var result = await _sut.AddItem(sessionId, request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AddItem_ReturnsNotFound_WhenProductNotFound()
    {
        // Arrange
        var sessionId = "session-123";
        var request = new AddToCartRequest(Guid.NewGuid(), 2);

        A.CallTo(() => _cartService.AddItemToCartAsync(sessionId, request))
.Throws(new KeyNotFoundException("Product not found"));

        // Act
        var result = await _sut.AddItem(sessionId, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task AddItem_ReturnsBadRequest_WhenInsufficientStock()
    {
        // Arrange
        var sessionId = "session-123";
        var request = new AddToCartRequest(Guid.NewGuid(), 100);

        A.CallTo(() => _cartService.AddItemToCartAsync(sessionId, request))
            .Throws(new InvalidOperationException("Insufficient stock"));

        // Act
        var result = await _sut.AddItem(sessionId, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddItem_ReturnsBadRequest_WhenQuantityIsInvalid()
    {
        // Arrange
        var sessionId = "session-123";
        var request = new AddToCartRequest(Guid.NewGuid(), 0);

        A.CallTo(() => _cartService.AddItemToCartAsync(sessionId, request))
          .Throws(new ArgumentException("Invalid quantity"));

        // Act
        var result = await _sut.AddItem(sessionId, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddItem_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var sessionId = "session-123";
        var productId = Guid.NewGuid();
        var request = new AddToCartRequest(productId, 3);
        var cart = new CartDto(Guid.NewGuid(), sessionId, new List<CartItemDto>(), 3, 30m, DateTime.UtcNow, DateTime.UtcNow);

        A.CallTo(() => _cartService.AddItemToCartAsync(sessionId, request)).Returns(cart);

        // Act
        await _sut.AddItem(sessionId, request);

        // Assert
        A.CallTo(() => _cartService.AddItemToCartAsync(sessionId, A<AddToCartRequest>.That.Matches(r =>
           r.ProductId == productId && r.Quantity == 3
            ))).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region UpdateItemQuantity Tests

    [Fact]
    public async Task UpdateItemQuantity_ReturnsOkResult_WithUpdatedCart()
    {
        // Arrange
        var sessionId = "session-123";
        var cartItemId = Guid.NewGuid();
        var request = new UpdateCartItemRequest(5);
        var cart = new CartDto(Guid.NewGuid(), sessionId, new List<CartItemDto>(), 5, 50m, DateTime.UtcNow, DateTime.UtcNow);

        A.CallTo(() => _cartService.UpdateCartItemQuantityAsync(sessionId, cartItemId, request.Quantity)).Returns(cart);

        // Act
        var result = await _sut.UpdateItemQuantity(sessionId, cartItemId, request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UpdateItemQuantity_ReturnsNotFound_WhenCartItemNotFound()
    {
        // Arrange
        var sessionId = "session-123";
        var cartItemId = Guid.NewGuid();
        var request = new UpdateCartItemRequest(5);

        A.CallTo(() => _cartService.UpdateCartItemQuantityAsync(sessionId, cartItemId, request.Quantity))
            .Throws(new KeyNotFoundException("Cart item not found"));

        // Act
        var result = await _sut.UpdateItemQuantity(sessionId, cartItemId, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateItemQuantity_ReturnsBadRequest_WhenQuantityExceedsStock()
    {
        // Arrange
        var sessionId = "session-123";
        var cartItemId = Guid.NewGuid();
        var request = new UpdateCartItemRequest(1000);

        A.CallTo(() => _cartService.UpdateCartItemQuantityAsync(sessionId, cartItemId, request.Quantity))
     .Throws(new InvalidOperationException("Insufficient stock"));

        // Act
        var result = await _sut.UpdateItemQuantity(sessionId, cartItemId, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateItemQuantity_ReturnsBadRequest_WhenQuantityIsInvalid()
    {
        // Arrange
        var sessionId = "session-123";
        var cartItemId = Guid.NewGuid();
        var request = new UpdateCartItemRequest(0);

        A.CallTo(() => _cartService.UpdateCartItemQuantityAsync(sessionId, cartItemId, request.Quantity))
          .Throws(new ArgumentException("Invalid quantity"));

        // Act
        var result = await _sut.UpdateItemQuantity(sessionId, cartItemId, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateItemQuantity_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var sessionId = "session-123";
        var cartItemId = Guid.NewGuid();
        var newQuantity = 7;
        var request = new UpdateCartItemRequest(newQuantity);
        var cart = new CartDto(Guid.NewGuid(), sessionId, new List<CartItemDto>(), 7, 70m, DateTime.UtcNow, DateTime.UtcNow);

        A.CallTo(() => _cartService.UpdateCartItemQuantityAsync(sessionId, cartItemId, newQuantity)).Returns(cart);

        // Act
        await _sut.UpdateItemQuantity(sessionId, cartItemId, request);

        // Assert
        A.CallTo(() => _cartService.UpdateCartItemQuantityAsync(sessionId, cartItemId, newQuantity))
            .MustHaveHappenedOnceExactly();
    }

    #endregion

    #region RemoveItem Tests

    [Fact]
    public async Task RemoveItem_ReturnsOkResult_WithUpdatedCart()
    {
        // Arrange
        var sessionId = "session-123";
        var cartItemId = Guid.NewGuid();
        var cart = new CartDto(Guid.NewGuid(), sessionId, new List<CartItemDto>(), 0, 0m, DateTime.UtcNow, DateTime.UtcNow);

        A.CallTo(() => _cartService.RemoveItemFromCartAsync(sessionId, cartItemId)).Returns(cart);

        // Act
        var result = await _sut.RemoveItem(sessionId, cartItemId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task RemoveItem_ReturnsNotFound_WhenCartItemNotFound()
    {
        // Arrange
        var sessionId = "session-123";
        var cartItemId = Guid.NewGuid();

        A.CallTo(() => _cartService.RemoveItemFromCartAsync(sessionId, cartItemId))
        .Throws(new KeyNotFoundException("Cart item not found"));

        // Act
        var result = await _sut.RemoveItem(sessionId, cartItemId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task RemoveItem_ReturnsBadRequest_WhenInvalidOperation()
    {
        // Arrange
        var sessionId = "session-123";
        var cartItemId = Guid.NewGuid();

        A.CallTo(() => _cartService.RemoveItemFromCartAsync(sessionId, cartItemId))
    .Throws(new InvalidOperationException("Cannot remove item"));

        // Act
        var result = await _sut.RemoveItem(sessionId, cartItemId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RemoveItem_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var sessionId = "session-123";
        var cartItemId = Guid.NewGuid();
        var cart = new CartDto(Guid.NewGuid(), sessionId, new List<CartItemDto>(), 0, 0m, DateTime.UtcNow, DateTime.UtcNow);

        A.CallTo(() => _cartService.RemoveItemFromCartAsync(sessionId, cartItemId)).Returns(cart);

        // Act
        await _sut.RemoveItem(sessionId, cartItemId);

        // Assert
        A.CallTo(() => _cartService.RemoveItemFromCartAsync(sessionId, cartItemId))
            .MustHaveHappenedOnceExactly();
    }

    #endregion

    #region ClearCart Tests

    [Fact]
    public async Task ClearCart_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var sessionId = "session-123";

        A.CallTo(() => _cartService.ClearCartAsync(sessionId)).Returns(true);

        // Act
        var result = await _sut.ClearCart(sessionId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ClearCart_ReturnsNotFound_WhenCartDoesNotExist()
    {
        // Arrange
        var sessionId = "non-existent";

        A.CallTo(() => _cartService.ClearCartAsync(sessionId))
       .Throws(new KeyNotFoundException("Cart not found"));

        // Act
        var result = await _sut.ClearCart(sessionId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ClearCart_ReturnsBadRequest_WhenInvalidOperation()
    {
        // Arrange
        var sessionId = "session-123";

        A.CallTo(() => _cartService.ClearCartAsync(sessionId))
            .Throws(new InvalidOperationException("Cannot clear cart"));

        // Act
        var result = await _sut.ClearCart(sessionId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ClearCart_CallsServiceWithCorrectSessionId()
    {
        // Arrange
        var sessionId = "session-456";

        A.CallTo(() => _cartService.ClearCartAsync(sessionId)).Returns(true);

        // Act
        await _sut.ClearCart(sessionId);

        // Assert
        A.CallTo(() => _cartService.ClearCartAsync(sessionId)).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region Status Code Tests

    [Fact]
    public async Task GetCart_ReturnsStatusCode200()
    {
        // Arrange
        var sessionId = "session-123";
        var cart = new CartDto(Guid.NewGuid(), sessionId, new List<CartItemDto>(), 0, 0m, DateTime.UtcNow, DateTime.UtcNow);

        A.CallTo(() => _cartService.GetOrCreateCartAsync(sessionId)).Returns(cart);

        // Act
        var result = await _sut.GetCart(sessionId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task AddItem_WithValidRequest_ReturnsStatusCode200()
    {
        // Arrange
        var sessionId = "session-123";
        var request = new AddToCartRequest(Guid.NewGuid(), 2);
        var cart = new CartDto(Guid.NewGuid(), sessionId, new List<CartItemDto>(), 2, 20m, DateTime.UtcNow, DateTime.UtcNow);

        A.CallTo(() => _cartService.AddItemToCartAsync(sessionId, request)).Returns(cart);

        // Act
        var result = await _sut.AddItem(sessionId, request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ClearCart_WhenSuccessful_ReturnsStatusCode204()
    {
        // Arrange
        var sessionId = "session-123";

        A.CallTo(() => _cartService.ClearCartAsync(sessionId)).Returns(true);

        // Act
        var result = await _sut.ClearCart(sessionId);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult!.StatusCode.Should().Be(204);
    }

    #endregion
}
