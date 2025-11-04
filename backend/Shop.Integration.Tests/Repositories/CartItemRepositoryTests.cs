using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;

namespace Shop.Integration.Tests.Repositories;

public class CartItemRepositoryTests : IDisposable
{
    private readonly ShopDbContext _context;
    private readonly CartItemRepository _sut;
    private readonly Guid _testCategoryId;
    private readonly Guid _testProductId;
    private readonly Guid _testCartId;

    public CartItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ShopDbContext>()
        .UseInMemoryDatabase($"test_db_{Guid.NewGuid()}")
            .Options;
        _context = new ShopDbContext(options);
   _sut = new CartItemRepository(_context);
        _testCategoryId = Guid.NewGuid();
        _testProductId = Guid.NewGuid();
   _testCartId = Guid.NewGuid();

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

var cart = new Cart
        {
        Id = _testCartId,
   SessionId = "test-session",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        _context.Products.Add(product);
        _context.Carts.Add(cart);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_WithNewCartItem_PersistsToDatabase()
    {
        // Arrange
        var cartItem = new CartItem
        {
    Id = Guid.NewGuid(),
            CartId = _testCartId,
          ProductId = _testProductId,
      Quantity = 2
        };

        // Act
        var result = await _sut.CreateAsync(cartItem);

        // Assert
        result.Should().NotBeNull();
        result.CartId.Should().Be(_testCartId);
      result.ProductId.Should().Be(_testProductId);
  result.Quantity.Should().Be(2);
        result.AddedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Product.Should().NotBeNull();
        result.Product.Category.Should().NotBeNull();

        // Verify it's in the database
        var dbItem = await _context.CartItems.FindAsync(cartItem.Id);
        dbItem.Should().NotBeNull();
  }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCartItem()
    {
        // Arrange
        var cartItemId = Guid.NewGuid();
  var cartItem = new CartItem
 {
    Id = cartItemId,
  CartId = _testCartId,
   ProductId = _testProductId,
            Quantity = 3,
AddedAt = DateTime.UtcNow
        };

        _context.CartItems.Add(cartItem);
      await _context.SaveChangesAsync();
        _context.Entry(cartItem).State = EntityState.Detached;

    // Act
  var result = await _sut.GetByIdAsync(cartItemId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(cartItemId);
        result.Quantity.Should().Be(3);
   result.Product.Should().NotBeNull();
        result.Product.Name.Should().Be("Test Product");
      result.Cart.Should().NotBeNull();
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
    public async Task UpdateAsync_WithExistingCartItem_UpdatesQuantity()
    {
        // Arrange
        var cartItemId = Guid.NewGuid();
        var cartItem = new CartItem
        {
            Id = cartItemId,
     CartId = _testCartId,
   ProductId = _testProductId,
            Quantity = 2,
            AddedAt = DateTime.UtcNow
      };

        _context.CartItems.Add(cartItem);
     await _context.SaveChangesAsync();
     _context.Entry(cartItem).State = EntityState.Detached;

        // Modify quantity
        cartItem.Quantity = 5;

 // Act
        var result = await _sut.UpdateAsync(cartItem);

        // Assert
        result.Should().NotBeNull();
        result.Quantity.Should().Be(5);

     // Verify in database
        var dbItem = await _context.CartItems.FindAsync(cartItemId);
        dbItem!.Quantity.Should().Be(5);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesCartItem()
    {
        // Arrange
  var cartItemId = Guid.NewGuid();
        var cartItem = new CartItem
    {
            Id = cartItemId,
   CartId = _testCartId,
   ProductId = _testProductId,
      Quantity = 1,
          AddedAt = DateTime.UtcNow
      };

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();

        // Act
     var result = await _sut.DeleteAsync(cartItemId);

        // Assert
result.Should().BeTrue();

 // Verify it's removed from database
        var dbItem = await _context.CartItems.FindAsync(cartItemId);
        dbItem.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByCartAndProductAsync_WithValidIds_ReturnsCartItem()
    {
        // Arrange
        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
      CartId = _testCartId,
        ProductId = _testProductId,
      Quantity = 2,
            AddedAt = DateTime.UtcNow
        };

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        _context.Entry(cartItem).State = EntityState.Detached;

     // Act
        var result = await _sut.GetByCartAndProductAsync(_testCartId, _testProductId);

    // Assert
     result.Should().NotBeNull();
        result!.CartId.Should().Be(_testCartId);
        result.ProductId.Should().Be(_testProductId);
        result.Quantity.Should().Be(2);
   result.Product.Should().NotBeNull();
        result.Product.Category.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByCartAndProductAsync_WithInvalidIds_ReturnsNull()
    {
     // Arrange
     var invalidCartId = Guid.NewGuid();
        var invalidProductId = Guid.NewGuid();

     // Act
        var result = await _sut.GetByCartAndProductAsync(invalidCartId, invalidProductId);

      // Assert
        result.Should().BeNull();
    }

 [Fact]
    public async Task GetByCartIdAsync_WithValidCartId_ReturnsAllItems()
    {
    // Arrange
        var product2 = new Product
        {
     Id = Guid.NewGuid(),
     Name = "Product 2",
         Price = 49.99m,
 Stock = 50,
       CategoryId = _testCategoryId
        };

  _context.Products.Add(product2);

      var items = new List<CartItem>
        {
   new()
            {
      Id = Guid.NewGuid(),
 CartId = _testCartId,
   ProductId = _testProductId,
          Quantity = 2,
 AddedAt = DateTime.UtcNow
       },
new()
      {
     Id = Guid.NewGuid(),
           CartId = _testCartId,
        ProductId = product2.Id,
    Quantity = 3,
      AddedAt = DateTime.UtcNow
  }
        };

        _context.CartItems.AddRange(items);
        await _context.SaveChangesAsync();
        items.ForEach(i => _context.Entry(i).State = EntityState.Detached);

        // Act
        var result = await _sut.GetByCartIdAsync(_testCartId);

    // Assert
    var itemList = result.ToList();
      itemList.Should().HaveCount(2);
        itemList.Should().Contain(i => i.ProductId == _testProductId && i.Quantity == 2);
        itemList.Should().Contain(i => i.ProductId == product2.Id && i.Quantity == 3);
        itemList.Should().OnlyContain(i => i.Product != null);
    itemList.Should().OnlyContain(i => i.Cart != null);
    }

    [Fact]
    public async Task GetByCartIdAsync_WithEmptyCart_ReturnsEmptyList()
    {
   // Arrange
        var emptyCartId = Guid.NewGuid();
        var emptyCart = new Cart
        {
      Id = emptyCartId,
  SessionId = "empty-cart",
  CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
};

        _context.Carts.Add(emptyCart);
        await _context.SaveChangesAsync();

        // Act
      var result = await _sut.GetByCartIdAsync(emptyCartId);

        // Assert
     result.Should().BeEmpty();
  }

    [Fact]
    public async Task DeleteByCartIdAsync_WithValidCartId_RemovesAllItems()
    {
        // Arrange
        var product2 = new Product
    {
      Id = Guid.NewGuid(),
          Name = "Product 2",
      Price = 49.99m,
       Stock = 50,
            CategoryId = _testCategoryId
        };

        _context.Products.Add(product2);

      var items = new List<CartItem>
        {
            new()
     {
         Id = Guid.NewGuid(),
    CartId = _testCartId,
             ProductId = _testProductId,
          Quantity = 2,
                AddedAt = DateTime.UtcNow
  },
      new()
   {
    Id = Guid.NewGuid(),
       CartId = _testCartId,
          ProductId = product2.Id,
           Quantity = 3,
             AddedAt = DateTime.UtcNow
          }
  };

        _context.CartItems.AddRange(items);
        await _context.SaveChangesAsync();

        // Act
      var result = await _sut.DeleteByCartIdAsync(_testCartId);

        // Assert
      result.Should().Be(2);

        // Verify all items removed
        var remainingItems = await _context.CartItems
       .Where(i => i.CartId == _testCartId)
.ToListAsync();
        remainingItems.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteByCartIdAsync_WithEmptyCart_ReturnsZero()
    {
 // Arrange
        var emptyCartId = Guid.NewGuid();
        var emptyCart = new Cart
        {
         Id = emptyCartId,
          SessionId = "empty-cart",
      CreatedAt = DateTime.UtcNow,
         UpdatedAt = DateTime.UtcNow
        };

        _context.Carts.Add(emptyCart);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.DeleteByCartIdAsync(emptyCartId);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task CreateAsync_SetsAddedAtTimestamp()
  {
   // Arrange
 var cartItem = new CartItem
   {
            Id = Guid.NewGuid(),
     CartId = _testCartId,
            ProductId = _testProductId,
    Quantity = 1
    };

        var beforeCreate = DateTime.UtcNow;

        // Act
      var result = await _sut.CreateAsync(cartItem);

        var afterCreate = DateTime.UtcNow;

    // Assert
        result.AddedAt.Should().BeOnOrAfter(beforeCreate);
result.AddedAt.Should().BeOnOrBefore(afterCreate);
    }

    [Fact]
    public async Task GetByCartAndProductAsync_LoadsNestedRelationships()
    {
        // Arrange
        var cartItem = new CartItem
        {
     Id = Guid.NewGuid(),
     CartId = _testCartId,
            ProductId = _testProductId,
     Quantity = 1,
      AddedAt = DateTime.UtcNow
        };

     _context.CartItems.Add(cartItem);
  await _context.SaveChangesAsync();
    _context.Entry(cartItem).State = EntityState.Detached;

    // Act
    var result = await _sut.GetByCartAndProductAsync(_testCartId, _testProductId);

        // Assert
   result.Should().NotBeNull();
        result!.Product.Should().NotBeNull();
        result.Product.Category.Should().NotBeNull();
        result.Product.Category!.Name.Should().Be("Test Category");
    }
}
