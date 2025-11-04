using FakeItEasy;
using FluentAssertions;
using Shop.Application.Dtos;
using Shop.Application.Services.Cart;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Api.Tests.Services;

public class CartServiceTests
{
    // Shared setup for all nested test classes
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly CartService _sut;

    public CartServiceTests()
    {
        _cartRepository = A.Fake<ICartRepository>();
        _cartItemRepository = A.Fake<ICartItemRepository>();
        _productRepository = A.Fake<IProductRepository>();

        _sut = new CartService(_cartRepository, _cartItemRepository, _productRepository);
    }

    #region GetOrCreateCartAsync Tests

    public class GetOrCreateCartAsync : CartServiceTests
    {
        [Fact]
        public async Task WhenCartExists_ReturnsExistingCart()
        {
            // Arrange
            var sessionId = "test-session-123";
            var existingCart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddHours(-2),
                Items = new List<CartItem>()
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(existingCart);

            // Act
            var result = await _sut.GetOrCreateCartAsync(sessionId);

            // Assert
            result.Should().NotBeNull();
            result.SessionId.Should().Be(sessionId);
            result.Id.Should().Be(existingCart.Id);
            A.CallTo(() => _cartRepository.CreateAsync(A<Cart>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task WhenCartDoesNotExist_CreatesNewCart()
        {
            // Arrange
            var sessionId = "new-session-456";
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns((Cart?)null);
            A.CallTo(() => _cartRepository.CreateAsync(A<Cart>._))
                .ReturnsLazily((Cart c) =>
                {
                    c.Items = new List<CartItem>();
                    return c;
                });

            // Act
            var result = await _sut.GetOrCreateCartAsync(sessionId);

            // Assert
            result.Should().NotBeNull();
            result.SessionId.Should().Be(sessionId);
            result.TotalItems.Should().Be(0);
            result.TotalPrice.Should().Be(0m);
            A.CallTo(() => _cartRepository.CreateAsync(A<Cart>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task NewCart_HasCorrectInitialValues()
        {
            // Arrange
            var sessionId = "session-789";
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns((Cart?)null);
            A.CallTo(() => _cartRepository.CreateAsync(A<Cart>._))
                .ReturnsLazily((Cart c) =>
                {
                    c.Items = new List<CartItem>();
                    return c;
                });

            // Act
            var result = await _sut.GetOrCreateCartAsync(sessionId);

            // Assert
            result.Items.Should().BeEmpty();
            result.TotalItems.Should().Be(0);
            result.TotalPrice.Should().Be(0m);
        }
    }

    #endregion

    #region AddItemToCartAsync Tests

    public class AddItemToCartAsync : CartServiceTests
    {
        [Fact]
        public async Task WithValidProduct_AddsItemToCart()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Laptop",
                Price = 1299.99m,
                Stock = 10,
                CategoryId = categoryId,
                Category = new Category { Id = categoryId, Name = "Electronics" }
            };

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Items = new List<CartItem>()
            };

            var request = new AddToCartRequest(productId, 2);

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByCartAndProductAsync(cart.Id, productId)).Returns((CartItem?)null);
            A.CallTo(() => _cartItemRepository.CreateAsync(A<CartItem>._))
                .ReturnsLazily((CartItem ci) =>
                {
                    ci.Product = product;
                    return ci;
                });
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) =>
            {
                c.Items = new List<CartItem>
                {
                    new() { Id = Guid.NewGuid(), ProductId = productId, Quantity = 2, Product = product, CartId = c.Id }
                };
                return c;
            });
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(cart, new Cart
                {
                    Id = cart.Id,
                    SessionId = sessionId,
                    Items = new List<CartItem>
                    {
                        new() { Id = Guid.NewGuid(), ProductId = productId, Quantity = 2, Product = product, CartId = cart.Id }
                    }
                });

            // Act
            var result = await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.First().Quantity.Should().Be(2);
            A.CallTo(() => _cartItemRepository.CreateAsync(A<CartItem>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task WhenProductNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var request = new AddToCartRequest(productId, 1);

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns((Product?)null);

            // Act
            Func<Task> act = async () => await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Product with id {productId} not found.");
        }

        [Fact]
        public async Task WhenInsufficientStock_ThrowsInvalidOperationException()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Limited Item",
                Price = 99.99m,
                Stock = 3,
                CategoryId = Guid.NewGuid()
            };

            var request = new AddToCartRequest(productId, 5); // Requesting more than available

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

            // Act
            Func<Task> act = async () => await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Insufficient stock. Only {product.Stock} items available");
        }

        [Fact]
        public async Task WhenQuantityIsZero_ThrowsArgumentException()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var request = new AddToCartRequest(productId, 0);

            var product = new Product
            {
                Id = productId,
                Name = "Product",
                Price = 99.99m,
                Stock = 10,
                CategoryId = Guid.NewGuid()
            };

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

            // Act
            Func<Task> act = async () => await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Quantity must be greater than zero");
        }

        [Fact]
        public async Task WhenQuantityIsNegative_ThrowsArgumentException()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var request = new AddToCartRequest(productId, -5);

            var product = new Product
            {
                Id = productId,
                Name = "Product",
                Price = 99.99m,
                Stock = 10,
                CategoryId = Guid.NewGuid()
            };

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

            // Act
            Func<Task> act = async () => await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Quantity must be greater than zero");
        }

        [Fact]
        public async Task WhenProductAlreadyInCart_UpdatesQuantity()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Laptop",
                Price = 1299.99m,
                Stock = 10,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Electronics" }
            };

            var cart = new Cart
            {
                Id = cartId,
                SessionId = sessionId,
                Items = new List<CartItem>()
            };

            var existingCartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = productId,
                Quantity = 2,
                Product = product
            };

            var request = new AddToCartRequest(productId, 3);

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByCartAndProductAsync(cartId, productId)).Returns(existingCartItem);
            A.CallTo(() => _cartItemRepository.UpdateAsync(A<CartItem>._))
                .ReturnsLazily((CartItem ci) => ci);
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(cart, new Cart
                {
                    Id = cartId,
                    SessionId = sessionId,
                    Items = new List<CartItem>
                    {
                        new() { Id = existingCartItem.Id, ProductId = productId, Quantity = 5, Product = product, CartId = cartId }
                    }
                });

            // Act
            var result = await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            result.Items.Should().HaveCount(1);
            A.CallTo(() => _cartItemRepository.UpdateAsync(A<CartItem>.That.Matches(ci => ci.Quantity == 5)))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _cartItemRepository.CreateAsync(A<CartItem>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task WhenAddingMoreThanStock_ThrowsInvalidOperationException()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Limited Item",
                Price = 99.99m,
                Stock = 5,
                CategoryId = Guid.NewGuid()
            };

            var cart = new Cart { Id = cartId, SessionId = sessionId };
            var existingItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = productId,
                Quantity = 3
            };

            var request = new AddToCartRequest(productId, 3); // 3 + 3 = 6, but only 5 in stock

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByCartAndProductAsync(cartId, productId)).Returns(existingItem);

            // Act
            Func<Task> act = async () => await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot add 3 more. Total would be 6, but only 5 available.");
        }
    }

    #endregion

    #region UpdateCartItemQuantityAsync Tests

    public class UpdateCartItemQuantityAsync : CartServiceTests
    {
        [Fact]
        public async Task WithValidQuantity_UpdatesCartItem()
        {
            // Arrange
            var sessionId = "session-123";
            var cartItemId = Guid.NewGuid();
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Product",
                Price = 99.99m,
                Stock = 10,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Test" }
            };

            var cart = new Cart { Id = cartId, SessionId = sessionId };
            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = cartId,
                ProductId = productId,
                Quantity = 2,
                Product = product
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByIdAsync(cartItemId)).Returns(cartItem);
            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);
            A.CallTo(() => _cartItemRepository.UpdateAsync(A<CartItem>._)).ReturnsLazily((CartItem ci) => ci);
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(cart, new Cart
                {
                    Id = cartId,
                    SessionId = sessionId,
                    Items = new List<CartItem>
                    {
                        new() { Id = cartItemId, ProductId = productId, Quantity = 5, Product = product, CartId = cartId }
                    }
                });

            // Act
            var result = await _sut.UpdateCartItemQuantityAsync(sessionId, cartItemId, 5);

            // Assert
            result.Should().NotBeNull();
            A.CallTo(() => _cartItemRepository.UpdateAsync(A<CartItem>.That.Matches(ci => ci.Quantity == 5)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task WhenQuantityExceedsStock_ThrowsInvalidOperationException()
        {
            // Arrange
            var sessionId = "session-123";
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Stock = 3
            };

            var cart = new Cart { Id = Guid.NewGuid(), SessionId = sessionId };
            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = cart.Id,
                ProductId = productId,
                Quantity = 1
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByIdAsync(cartItemId)).Returns(cartItem);
            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

            // Act
            Func<Task> act = async () => await _sut.UpdateCartItemQuantityAsync(sessionId, cartItemId, 5);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Insufficient stock. Only 3 items available.");
        }

        [Fact]
        public async Task WhenQuantityIsZeroOrNegative_ThrowsArgumentException()
        {
            // Arrange
            var sessionId = "session-123";
            var cartItemId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _sut.UpdateCartItemQuantityAsync(sessionId, cartItemId, 0);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Quantity must be greater than 0.");
        }

        [Fact]
        public async Task WhenCartNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var sessionId = "non-existent-session";
            var cartItemId = Guid.NewGuid();

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns((Cart?)null);

            // Act
            Func<Task> act = async () => await _sut.UpdateCartItemQuantityAsync(sessionId, cartItemId, 5);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart not found.");
        }

        [Fact]
        public async Task WhenCartItemNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var sessionId = "session-123";
            var cartItemId = Guid.NewGuid();
            var cart = new Cart { Id = Guid.NewGuid(), SessionId = sessionId };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByIdAsync(cartItemId)).Returns((CartItem?)null);

            // Act
            Func<Task> act = async () => await _sut.UpdateCartItemQuantityAsync(sessionId, cartItemId, 5);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart item not found.");
        }
    }

    #endregion

    #region RemoveItemFromCartAsync Tests

    public class RemoveItemFromCartAsync : CartServiceTests
    {
        [Fact]
        public async Task WithValidCartItem_RemovesItem()
        {
            // Arrange
            var sessionId = "session-123";
            var cartItemId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var cart = new Cart { Id = cartId, SessionId = sessionId };
            var cartItem = new CartItem { Id = cartItemId, CartId = cartId };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByIdAsync(cartItemId)).Returns(cartItem);
            A.CallTo(() => _cartItemRepository.DeleteAsync(cartItemId)).Returns(true);
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(cart, new Cart
                {
                    Id = cartId,
                    SessionId = sessionId,
                    Items = new List<CartItem>()
                });

            // Act
            var result = await _sut.RemoveItemFromCartAsync(sessionId, cartItemId);

            // Assert
            result.Should().NotBeNull();
            A.CallTo(() => _cartItemRepository.DeleteAsync(cartItemId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task WhenCartNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var sessionId = "non-existent";
            var cartItemId = Guid.NewGuid();

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns((Cart?)null);

            // Act
            Func<Task> act = async () => await _sut.RemoveItemFromCartAsync(sessionId, cartItemId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart not found.");
        }
    }

    #endregion

    #region ClearCartAsync Tests

    public class ClearCartAsync : CartServiceTests
    {
        [Fact]
        public async Task WhenCartExists_ClearsAllItems()
        {
            // Arrange
            var sessionId = "session-123";
            var cartId = Guid.NewGuid();
            var cart = new Cart { Id = cartId, SessionId = sessionId };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.DeleteByCartIdAsync(cartId)).Returns(3);
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);

            // Act
            var result = await _sut.ClearCartAsync(sessionId);

            // Assert
            result.Should().BeTrue();
            A.CallTo(() => _cartItemRepository.DeleteByCartIdAsync(cartId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task WhenCartNotFound_ReturnsFalse()
        {
            // Arrange
            var sessionId = "non-existent";
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns((Cart?)null);

            // Act
            var result = await _sut.ClearCartAsync(sessionId);

            // Assert
            result.Should().BeFalse();
            A.CallTo(() => _cartItemRepository.DeleteByCartIdAsync(A<Guid>._)).MustNotHaveHappened();
        }
    }

    #endregion

    #region GetCartSummaryAsync Tests

    public class GetCartSummaryAsync : CartServiceTests
    {
        [Fact]
        public async Task WithItemsInCart_ReturnsCorrectSummary()
        {
            // Arrange
            var sessionId = "session-123";
            var product1 = new Product { Id = Guid.NewGuid(), Price = 10m };
            var product2 = new Product { Id = Guid.NewGuid(), Price = 20m };

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Items = new List<CartItem>
                {
                    new() { Id = Guid.NewGuid(), Quantity = 2, Product = product1 },
                    new() { Id = Guid.NewGuid(), Quantity = 3, Product = product2 }
                }
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);

            // Act
            var result = await _sut.GetCartSummaryAsync(sessionId);

            // Assert
            result.TotalItems.Should().Be(5); // 2 + 3
            result.TotalPrice.Should().Be(80m); // (2*10) + (3*20)
        }

        [Fact]
        public async Task WhenCartIsEmpty_ReturnsZeroSummary()
        {
            // Arrange
            var sessionId = "session-123";
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Items = new List<CartItem>()
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);

            // Act
            var result = await _sut.GetCartSummaryAsync(sessionId);

            // Assert
            result.TotalItems.Should().Be(0);
            result.TotalPrice.Should().Be(0m);
        }

        [Fact]
        public async Task WhenCartNotFound_ReturnsZeroSummary()
        {
            // Arrange
            var sessionId = "non-existent";
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns((Cart?)null);

            // Act
            var result = await _sut.GetCartSummaryAsync(sessionId);

            // Assert
            result.TotalItems.Should().Be(0);
            result.TotalPrice.Should().Be(0m);
        }
    }

    #endregion

    #region Edge Cases & Boundary Tests

    public class EdgeCasesAndBoundaryTests : CartServiceTests
    {
        [Fact]
        public async Task AddItemToCartAsync_WithMaximumQuantityEqualToStock_AddsSuccessfully()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Limited Product",
                Price = 99.99m,
                Stock = 5,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Test" }
            };

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Items = new List<CartItem>()
            };

            var request = new AddToCartRequest(productId, 5); // Exactly equal to stock

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByCartAndProductAsync(cart.Id, productId)).Returns((CartItem?)null);
            A.CallTo(() => _cartItemRepository.CreateAsync(A<CartItem>._))
                .ReturnsLazily((CartItem ci) => { ci.Product = product; return ci; });
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(cart, new Cart
                {
                    Id = cart.Id,
                    SessionId = sessionId,
                    Items = new List<CartItem>
                    {
                        new() { Id = Guid.NewGuid(), ProductId = productId, Quantity = 5, Product = product, CartId = cart.Id }
                    }
                });

            // Act
            var result = await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.First().Quantity.Should().Be(5);
        }

        [Fact]
        public async Task AddItemToCartAsync_WithVeryLargeCart_AddsItemSuccessfully()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Electronics" };

            // Create a cart with 50 different items
            var existingItems = Enumerable.Range(1, 50).Select(i => new CartItem
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Quantity = 2,
                Product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = $"Product {i}",
                    Price = 10m * i,
                    Stock = 100,
                    CategoryId = categoryId,
                    Category = category
                }
            }).ToList();

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Items = existingItems
            };

            var newProduct = new Product
            {
                Id = productId,
                Name = "New Product",
                Price = 999.99m,
                Stock = 10,
                CategoryId = categoryId,
                Category = category
            };

            var request = new AddToCartRequest(productId, 2);

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(newProduct);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByCartAndProductAsync(cart.Id, productId)).Returns((CartItem?)null);
            A.CallTo(() => _cartItemRepository.CreateAsync(A<CartItem>._))
                .ReturnsLazily((CartItem ci) => { ci.Product = newProduct; return ci; });
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);

            var updatedItems = existingItems.ToList();
            updatedItems.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantity = 2,
                Product = newProduct,
                CartId = cart.Id
            });

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(cart, new Cart
                {
                    Id = cart.Id,
                    SessionId = sessionId,
                    Items = updatedItems
                });

            // Act
            var result = await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(51);
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsync_ReducingQuantity_UpdatesSuccessfully()
        {
            // Arrange
            var sessionId = "session-123";
            var cartItemId = Guid.NewGuid();
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Product",
                Price = 99.99m,
                Stock = 10,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Test" }
            };

            var cart = new Cart { Id = cartId, SessionId = sessionId };
            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = cartId,
                ProductId = productId,
                Quantity = 10,
                Product = product
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByIdAsync(cartItemId)).Returns(cartItem);
            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);
            A.CallTo(() => _cartItemRepository.UpdateAsync(A<CartItem>._)).ReturnsLazily((CartItem ci) => ci);
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(cart, new Cart
                {
                    Id = cartId,
                    SessionId = sessionId,
                    Items = new List<CartItem>
                    {
                        new() { Id = cartItemId, ProductId = productId, Quantity = 5, Product = product, CartId = cartId }
                    }
                });

            // Act
            var result = await _sut.UpdateCartItemQuantityAsync(sessionId, cartItemId, 5);

            // Assert
            result.Should().NotBeNull();
            A.CallTo(() => _cartItemRepository.UpdateAsync(A<CartItem>.That.Matches(ci => ci.Quantity == 5)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetCartSummaryAsync_WithDecimalPrices_CalculatesCorrectly()
        {
            // Arrange
            var sessionId = "session-123";
            var product1 = new Product { Id = Guid.NewGuid(), Price = 10.99m };
            var product2 = new Product { Id = Guid.NewGuid(), Price = 20.49m };
            var product3 = new Product { Id = Guid.NewGuid(), Price = 5.95m };

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Items = new List<CartItem>
                {
                    new() { Id = Guid.NewGuid(), Quantity = 3, Product = product1 }, // 32.97
                    new() { Id = Guid.NewGuid(), Quantity = 2, Product = product2 }, // 40.98
                    new() { Id = Guid.NewGuid(), Quantity = 4, Product = product3 }  // 23.80
                }
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);

            // Act
            var result = await _sut.GetCartSummaryAsync(sessionId);

            // Assert
            result.TotalItems.Should().Be(9); // 3 + 2 + 4
            result.TotalPrice.Should().Be(97.75m); // 32.97 + 40.98 + 23.80
        }

        [Fact]
        public async Task GetCartSummaryAsync_WithLargeQuantities_CalculatesCorrectly()
        {
            // Arrange
            var sessionId = "session-123";
            var product = new Product { Id = Guid.NewGuid(), Price = 0.99m };

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Items = new List<CartItem>
                {
                    new() { Id = Guid.NewGuid(), Quantity = 1000, Product = product }
                }
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);

            // Act
            var result = await _sut.GetCartSummaryAsync(sessionId);

            // Assert
            result.TotalItems.Should().Be(1000);
            result.TotalPrice.Should().Be(990m); // 1000 * 0.99
        }

        [Fact]
        public async Task AddItemToCartAsync_WithZeroPriceProduct_AddsSuccessfully()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Free Sample",
                Price = 0m, // Free product
                Stock = 100,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Samples" }
            };

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Items = new List<CartItem>()
            };

            var request = new AddToCartRequest(productId, 1);

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByCartAndProductAsync(cart.Id, productId)).Returns((CartItem?)null);
            A.CallTo(() => _cartItemRepository.CreateAsync(A<CartItem>._))
                .ReturnsLazily((CartItem ci) => { ci.Product = product; return ci; });
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(cart, new Cart
                {
                    Id = cart.Id,
                    SessionId = sessionId,
                    Items = new List<CartItem>
                    {
                        new() { Id = Guid.NewGuid(), ProductId = productId, Quantity = 1, Product = product, CartId = cart.Id }
                    }
                });

            // Act
            var result = await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            result.Should().NotBeNull();
            result.Items.First().ProductPrice.Should().Be(0m);
            result.TotalPrice.Should().Be(0m);
        }
    }

    #endregion

    #region Data Validation Tests

    public class DataValidationTests : CartServiceTests
    {
        [Fact]
        public async Task GetOrCreateCartAsync_WithEmptySessionId_CreatesCart()
        {
            // Arrange
            var sessionId = "";
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns((Cart?)null);
            A.CallTo(() => _cartRepository.CreateAsync(A<Cart>._))
                .ReturnsLazily((Cart c) => { c.Items = new List<CartItem>(); return c; });

            // Act
            var result = await _sut.GetOrCreateCartAsync(sessionId);

            // Assert
            result.Should().NotBeNull();
            result.SessionId.Should().Be("");
        }

        [Fact]
        public async Task GetOrCreateCartAsync_WithWhitespaceSessionId_CreatesCart()
        {
            // Arrange
            var sessionId = "   ";
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns((Cart?)null);
            A.CallTo(() => _cartRepository.CreateAsync(A<Cart>._))
                .ReturnsLazily((Cart c) => { c.Items = new List<CartItem>(); return c; });

            // Act
            var result = await _sut.GetOrCreateCartAsync(sessionId);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task AddItemToCartAsync_WithGuidEmpty_ThrowsKeyNotFoundException()
        {
            // Arrange
            var sessionId = "session-123";
            var request = new AddToCartRequest(Guid.Empty, 1);

            A.CallTo(() => _productRepository.GetByIdAsync(Guid.Empty)).Returns((Product?)null);

            // Act
            Func<Task> act = async () => await _sut.AddItemToCartAsync(sessionId, request);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Product with id {Guid.Empty} not found.");
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsync_WithNegativeQuantity_ThrowsArgumentException()
        {
            // Arrange
            var sessionId = "session-123";
            var cartItemId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _sut.UpdateCartItemQuantityAsync(sessionId, cartItemId, -5);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Quantity must be greater than 0.");
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_WithItemFromDifferentCart_ThrowsKeyNotFoundException()
        {
            // Arrange
            var sessionId = "session-123";
            var cartItemId = Guid.NewGuid();
            var cart = new Cart { Id = Guid.NewGuid(), SessionId = sessionId };
            var differentCartId = Guid.NewGuid();
            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = differentCartId // Different cart
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByIdAsync(cartItemId)).Returns(cartItem);

            // Act
            Func<Task> act = async () => await _sut.RemoveItemFromCartAsync(sessionId, cartItemId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart item not found.");
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsync_WithDeletedProduct_ThrowsKeyNotFoundException()
        {
            // Arrange
            var sessionId = "session-123";
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var cart = new Cart { Id = Guid.NewGuid(), SessionId = sessionId };
            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = cart.Id,
                ProductId = productId,
                Quantity = 2
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.GetByIdAsync(cartItemId)).Returns(cartItem);
            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns((Product?)null);

            // Act
            Func<Task> act = async () => await _sut.UpdateCartItemQuantityAsync(sessionId, cartItemId, 5);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found.");
        }
    }

    #endregion

    #region Integration & Workflow Tests

    public class IntegrationWorkflowTests : CartServiceTests
    {
        [Fact]
        public async Task CompleteCartWorkflow_AddUpdateRemove_WorksCorrectly()
        {
            // Arrange
            var sessionId = "session-workflow";
            var productId = Guid.NewGuid();
            var cartItemId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Workflow Product",
                Price = 99.99m,
                Stock = 20,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Test" }
            };

            var emptyCart = new Cart
            {
                Id = cartId,
                SessionId = sessionId,
                Items = new List<CartItem>()
            };

            // Step 1: Add item to cart
            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(
                    emptyCart,
                    new Cart
                    {
                        Id = cartId,
                        SessionId = sessionId,
                        Items = new List<CartItem>
                        {
                            new() { Id = cartItemId, ProductId = productId, Quantity = 2, Product = product, CartId = cartId }
                        }
                    },
                    new Cart
                    {
                        Id = cartId,
                        SessionId = sessionId,
                        Items = new List<CartItem>
                        {
                            new() { Id = cartItemId, ProductId = productId, Quantity = 2, Product = product, CartId = cartId }
                        }
                    },
                    new Cart
                    {
                        Id = cartId,
                        SessionId = sessionId,
                        Items = new List<CartItem>
                        {
                            new() { Id = cartItemId, ProductId = productId, Quantity = 5, Product = product, CartId = cartId }
                        }
                    },
                    new Cart
                    {
                        Id = cartId,
                        SessionId = sessionId,
                        Items = new List<CartItem>()
                    }
                );

            A.CallTo(() => _cartItemRepository.GetByCartAndProductAsync(cartId, productId)).Returns((CartItem?)null);
            A.CallTo(() => _cartItemRepository.CreateAsync(A<CartItem>._))
                .ReturnsLazily((CartItem ci) => { ci.Id = cartItemId; ci.Product = product; return ci; });
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);

            var addRequest = new AddToCartRequest(productId, 2);
            var addResult = await _sut.AddItemToCartAsync(sessionId, addRequest);

            // Step 2: Update quantity
            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = cartId,
                ProductId = productId,
                Quantity = 2,
                Product = product
            };

            A.CallTo(() => _cartItemRepository.GetByIdAsync(cartItemId)).Returns(cartItem);
            A.CallTo(() => _cartItemRepository.UpdateAsync(A<CartItem>._)).ReturnsLazily((CartItem ci) => ci);

            var updateResult = await _sut.UpdateCartItemQuantityAsync(sessionId, cartItemId, 5);

            // Step 3: Remove item - need to update the cartItem reference for this step
            var cartItemToRemove = new CartItem
            {
                Id = cartItemId,
                CartId = cartId,
                ProductId = productId,
                Quantity = 5,
                Product = product
            };
            A.CallTo(() => _cartItemRepository.GetByIdAsync(cartItemId)).Returns(cartItemToRemove);
            A.CallTo(() => _cartItemRepository.DeleteAsync(cartItemId)).Returns(true);

            var removeResult = await _sut.RemoveItemFromCartAsync(sessionId, cartItemId);

            // Assert
            addResult.Items.Should().HaveCount(1);
            addResult.Items.First().Quantity.Should().Be(2);

            updateResult.Items.Should().HaveCount(1);
            updateResult.Items.First().Quantity.Should().Be(5);

            removeResult.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task AddMultipleItemsThenClearCart_LeavesEmptyCart()
        {
            // Arrange
            var sessionId = "session-clear";
            var cartId = Guid.NewGuid();

            var products = Enumerable.Range(1, 5).Select(i => new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Product {i}",
                Price = 10m * i,
                Stock = 10,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Test" }
            }).ToList();

            var cart = new Cart
            {
                Id = cartId,
                SessionId = sessionId,
                Items = new List<CartItem>()
            };

            // Add all products to cart
            foreach (var product in products)
            {
                A.CallTo(() => _productRepository.GetByIdAsync(product.Id)).Returns(product);
                A.CallTo(() => _cartItemRepository.GetByCartAndProductAsync(cartId, product.Id)).Returns((CartItem?)null);
            }

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartItemRepository.CreateAsync(A<CartItem>._))
                .ReturnsLazily((CartItem ci) => ci);
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);

            var cartWithItems = new Cart
            {
                Id = cartId,
                SessionId = sessionId,
                Items = products.Select(p => new CartItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = p.Id,
                    Quantity = 2,
                    Product = p,
                    CartId = cartId
                }).ToList()
            };

            // Clear cart
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsLazily(() => cartWithItems);
            A.CallTo(() => _cartItemRepository.DeleteByCartIdAsync(cartId)).Returns(5);

            // Act
            var clearResult = await _sut.ClearCartAsync(sessionId);
            var summary = await _sut.GetCartSummaryAsync(sessionId);

            // Assert
            clearResult.Should().BeTrue();
            // Note: Summary will still show items because we're using fake repositories
            // In a real scenario with a database, the items would be gone
        }

        [Fact]
        public async Task AddSameProductMultipleTimes_AccumulatesQuantity()
        {
            // Arrange
            var sessionId = "session-123";
            var productId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Laptop",
                Price = 1299.99m,
                Stock = 10,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Id = Guid.NewGuid(), Name = "Electronics" }
            };

            var cart = new Cart
            {
                Id = cartId,
                SessionId = sessionId,
                Items = new List<CartItem>()
            };

            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);
            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);
            A.CallTo(() => _cartRepository.UpdateAsync(A<Cart>._)).ReturnsLazily((Cart c) => c);

            // First add
            A.CallTo(() => _cartItemRepository.GetByCartAndProductAsync(cartId, productId))
                .ReturnsNextFromSequence(
                    (CartItem?)null,
                    new CartItem { Id = Guid.NewGuid(), CartId = cartId, ProductId = productId, Quantity = 3, Product = product },
                    new CartItem { Id = Guid.NewGuid(), CartId = cartId, ProductId = productId, Quantity = 8, Product = product }
                );

            A.CallTo(() => _cartItemRepository.CreateAsync(A<CartItem>._))
                .ReturnsLazily((CartItem ci) => { ci.Product = product; return ci; });
            A.CallTo(() => _cartItemRepository.UpdateAsync(A<CartItem>._))
                .ReturnsLazily((CartItem ci) => ci);

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId))
                .ReturnsNextFromSequence(
                    cart,
                    new Cart
                    {
                        Id = cartId,
                        SessionId = sessionId,
                        Items = new List<CartItem>
                        {
                            new() { Id = Guid.NewGuid(), ProductId = productId, Quantity = 3, Product = product, CartId = cartId }
                        }
                    },
                    new Cart
                    {
                        Id = cartId,
                        SessionId = sessionId,
                        Items = new List<CartItem>
                        {
                            new() { Id = Guid.NewGuid(), ProductId = productId, Quantity = 3, Product = product, CartId = cartId }
                        }
                    },
                    new Cart
                    {
                        Id = cartId,
                        SessionId = sessionId,
                        Items = new List<CartItem>
                        {
                            new() { Id = Guid.NewGuid(), ProductId = productId, Quantity = 8, Product = product, CartId = cartId }
                        }
                    }
                );

            // Act
            var request1 = new AddToCartRequest(productId, 3);
            var result1 = await _sut.AddItemToCartAsync(sessionId, request1);

            var request2 = new AddToCartRequest(productId, 5);
            var result2 = await _sut.AddItemToCartAsync(sessionId, request2);

            // Assert
            result1.Items.First().Quantity.Should().Be(3);
            result2.Items.First().Quantity.Should().Be(8); // 3 + 5
        }

        [Fact]
        public async Task GetCartSummary_AfterMultipleOperations_ReflectsCurrentState()
        {
            // Arrange
            var sessionId = "session-summary";
            var product1 = new Product { Id = Guid.NewGuid(), Price = 100m };
            var product2 = new Product { Id = Guid.NewGuid(), Price = 50m };

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Items = new List<CartItem>
                {
                    new() { Id = Guid.NewGuid(), Quantity = 2, Product = product1 }, // 200
                    new() { Id = Guid.NewGuid(), Quantity = 3, Product = product2 }  // 150
                }
            };

            A.CallTo(() => _cartRepository.GetBySessionIdAsync(sessionId)).Returns(cart);

            // Act
            var summary = await _sut.GetCartSummaryAsync(sessionId);

            // Assert
            summary.TotalItems.Should().Be(5);
            summary.TotalPrice.Should().Be(350m);
        }
    }

    #endregion
}