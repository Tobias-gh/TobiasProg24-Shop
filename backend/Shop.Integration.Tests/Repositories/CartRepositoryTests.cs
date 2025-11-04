using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;

namespace Shop.Integration.Tests.Repositories;

public class CartRepositoryTests : IDisposable
{
    private readonly ShopDbContext _context;
    private readonly CartRepository _sut;
    private readonly Guid _testCategoryId;
    private readonly Guid _testProductId;

    public CartRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ShopDbContext>()
  .UseInMemoryDatabase($"test_db_{Guid.NewGuid()}")
       .Options;
        _context = new ShopDbContext(options);
        _sut = new CartRepository(_context);
        _testCategoryId = Guid.NewGuid();
        _testProductId = Guid.NewGuid();

        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new Category
        {
            Id = _testCategoryId,
            Name = "Test Category",
            Description = "Category for testing"
        };

        var product = new Product
        {
            Id = _testProductId,
            Name = "Test Product",
            Description = "Product for testing",
            Price = 99.99m,
            Stock = 100,
            CategoryId = _testCategoryId
        };

        _context.Categories.Add(category);
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetBySessionIdAsync_WithValidSession_ReturnsCartWithItems()
    {
        // Arrange
        var sessionId = "test-session-123";
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            CartId = cart.Id,
            ProductId = _testProductId,
            Quantity = 2,
            AddedAt = DateTime.UtcNow
        };

        _context.Carts.Add(cart);
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        _context.Entry(cart).State = EntityState.Detached;
        _context.Entry(cartItem).State = EntityState.Detached;

        // Act
        var result = await _sut.GetBySessionIdAsync(sessionId);

        // Assert
        result.Should().NotBeNull();
        result!.SessionId.Should().Be(sessionId);
        result.Items.Should().HaveCount(1);
        result.Items.First().ProductId.Should().Be(_testProductId);
        result.Items.First().Quantity.Should().Be(2);
        result.Items.First().Product.Should().NotBeNull();
        result.Items.First().Product.Category.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBySessionIdAsync_WithInvalidSession_ReturnsNull()
    {
        // Arrange
        var nonExistentSession = "non-existent-session";

        // Act
        var result = await _sut.GetBySessionIdAsync(nonExistentSession);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithNewCart_PersistsToDatabase()
    {
        // Arrange
        var sessionId = "new-session-456";
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId
        };

        // Act
        var result = await _sut.CreateAsync(cart);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(sessionId);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify it's actually in the database
        var dbCart = await _context.Carts.FindAsync(cart.Id);
        dbCart.Should().NotBeNull();
        dbCart!.SessionId.Should().Be(sessionId);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingCart_UpdatesTimestamp()
    {
        // Arrange
        var sessionId = "update-session-789";
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
        _context.Entry(cart).State = EntityState.Detached;

        var oldUpdatedAt = cart.UpdatedAt;
        await Task.Delay(100); // Small delay to ensure timestamp difference

        // Act
        var result = await _sut.UpdateAsync(cart);

        // Assert
        result.Should().NotBeNull();
        result.UpdatedAt.Should().BeAfter(oldUpdatedAt);
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCartWithItems()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var sessionId = "session-getbyid";
        var cart = new Cart
        {
            Id = cartId,
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            CartId = cartId,
            ProductId = _testProductId,
            Quantity = 3,
            AddedAt = DateTime.UtcNow
        };

        _context.Carts.Add(cart);
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        _context.Entry(cart).State = EntityState.Detached;
        _context.Entry(cartItem).State = EntityState.Detached;

        // Act
        var result = await _sut.GetByIdAsync(cartId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(cartId);
        result.SessionId.Should().Be(sessionId);
        result.Items.Should().HaveCount(1);
        result.Items.First().Quantity.Should().Be(3);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySessionIdAsync_WithEmptyCart_ReturnsCartWithNoItems()
    {
        // Arrange
        var sessionId = "empty-cart-session";
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
        _context.Entry(cart).State = EntityState.Detached;

        // Act
        var result = await _sut.GetBySessionIdAsync(sessionId);

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBySessionIdAsync_WithMultipleItems_LoadsAllItems()
    {
        // Arrange
        var sessionId = "multi-item-session";
        var cartId = Guid.NewGuid();

        // Create additional products
        var product2 = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Product 2",
            Price = 49.99m,
            Stock = 50,
            CategoryId = _testCategoryId
        };

        var product3 = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Product 3",
            Price = 29.99m,
            Stock = 75,
            CategoryId = _testCategoryId
        };

        _context.Products.AddRange(product2, product3);

        var cart = new Cart
        {
            Id = cartId,
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var items = new List<CartItem>
        {
            new()
          {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = _testProductId,
                Quantity = 1,
                AddedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = product2.Id,
                Quantity = 2,
                AddedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = product3.Id,
                Quantity = 3,
                AddedAt = DateTime.UtcNow
            }
};

        _context.Carts.Add(cart);
        _context.CartItems.AddRange(items);
        await _context.SaveChangesAsync();
        _context.Entry(cart).State = EntityState.Detached;
        items.ForEach(i => _context.Entry(i).State = EntityState.Detached);

        // Act
        var result = await _sut.GetBySessionIdAsync(sessionId);

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(3);
        result.Items.Should().Contain(i => i.ProductId == _testProductId && i.Quantity == 1);
        result.Items.Should().Contain(i => i.ProductId == product2.Id && i.Quantity == 2);
        result.Items.Should().Contain(i => i.ProductId == product3.Id && i.Quantity == 3);
    }

    [Fact]
    public async Task CreateAsync_SetsTimestampsAutomatically()
    {
        // Arrange
        var sessionId = "timestamp-test";
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId
        };

        var beforeCreate = DateTime.UtcNow;

        // Act
        var result = await _sut.CreateAsync(cart);

        var afterCreate = DateTime.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        result.CreatedAt.Should().BeOnOrBefore(afterCreate);
        result.UpdatedAt.Should().BeOnOrAfter(beforeCreate);
        result.UpdatedAt.Should().BeOnOrBefore(afterCreate);
    }

    [Fact]
    public async Task GetBySessionIdAsync_IncludesProductCategories_ForNestedRelationships()
    {
        // Arrange
        var sessionId = "nested-session";
        var cartId = Guid.NewGuid();

        var cart = new Cart
        {
            Id = cartId,
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            CartId = cartId,
            ProductId = _testProductId,
            Quantity = 1,
            AddedAt = DateTime.UtcNow
        };

        _context.Carts.Add(cart);
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        _context.Entry(cart).State = EntityState.Detached;
        _context.Entry(cartItem).State = EntityState.Detached;

        // Act
        var result = await _sut.GetBySessionIdAsync(sessionId);

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);

        var item = result.Items.First();
        item.Product.Should().NotBeNull();
        item.Product.Category.Should().NotBeNull();
        item.Product.Category!.Name.Should().Be("Test Category");
    }
}
