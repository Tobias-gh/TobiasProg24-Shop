using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Integration.Tests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ShopDbContext _context;
    private readonly ProductRepository _sut;
    private readonly Guid _testCategoryId;

    public void SeedTestData()
    {
        var category = new Category
        {
            Id = _testCategoryId,
            Name = "Test Category",
            Description = "A category for testing purposes"
        };
        _context.Categories.Add(category);
        _context.SaveChanges();
    }

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ShopDbContext>()
            .UseInMemoryDatabase($"test_db_{Guid.NewGuid()}")
            .Options;
            _context = new ShopDbContext(options);
            _sut = new ProductRepository(_context);
            _testCategoryId = Guid.NewGuid();

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProductWithCategory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "A product for testing",
            Price = 9.99m,
            Stock = 100,
            CategoryId = _testCategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _context.Entry(product).State = EntityState.Detached;

        // Act
        var result = await _sut.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(9.99m);
        result.Stock.Should().Be(100);
        result.Category.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleProducts_ReturnsAllWithCategories()
    {
        // Arrange
        var products = new List<Product>
        {
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            Description = "Business laptop",
            Price = 899.99m,
            Stock = 10,
            CategoryId = _testCategoryId
        },
        new()
    {
            Id = Guid.NewGuid(),
            Name = "Mouse",
            Description = "Wireless mouse",
            Price = 29.99m,
            Stock = 50,
            CategoryId = _testCategoryId
         },
        new()
            {
            Id = Guid.NewGuid(),
            Name = "Keyboard",
            Description = "Mechanical keyboard",
            Price = 79.99m,
            Stock = 25,
            CategoryId = _testCategoryId
     }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
        products.ForEach(p => _context.Entry(p).State = EntityState.Detached);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        var productList = result.ToList();
        productList.Should().HaveCount(3);
        productList.Should().Contain(p => p.Name == "Laptop");
        productList.Should().Contain(p => p.Name == "Mouse");
        productList.Should().Contain(p => p.Name == "Keyboard");
        productList.Should().OnlyContain(p => p.Category != null);
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
    public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange - Database only has test category, no products

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WithNullDescription_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Simple Product",
            Description = null,
            Price = 49.99m,
            Stock = 20,
            CategoryId = _testCategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _context.Entry(product).State = EntityState.Detached;

        // Act
        var result = await _sut.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().BeNull();
        result.Name.Should().Be("Simple Product");
    }

    [Fact]
    public async Task GetByIdAsync_WithZeroPrice_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Free Product",
            Description = "This is free",
            Price = 0m,
            Stock = 1000,
            CategoryId = _testCategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _context.Entry(product).State = EntityState.Detached;

        // Act
        var result = await _sut.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Price.Should().Be(0m);
        result.Stock.Should().Be(1000);
    }

    [Fact]
    public async Task GetByIdAsync_WithZeroStock_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Out of Stock Product",
            Description = "Currently unavailable",
            Price = 99.99m,
            Stock = 0,
            CategoryId = _testCategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _context.Entry(product).State = EntityState.Detached;

        // Act
        var result = await _sut.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Stock.Should().Be(0);
        result.Name.Should().Be("Out of Stock Product");
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleCategories_ReturnsProductsWithCorrectCategories()
    {
        // Arrange
        var category1 = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
        var category2 = new Category { Id = Guid.NewGuid(), Name = "Books" };
        _context.Categories.AddRange(category1, category2);

        var products = new List<Product>
        {
         new()
        {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Price = 999m,
                Stock = 5,
                CategoryId = category1.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Phone",
                Price = 699m,
                Stock = 10,
                CategoryId = category1.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Novel",
                Price = 19.99m,
                Stock = 100,
                CategoryId = category2.Id
            }
      };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
        products.ForEach(p => _context.Entry(p).State = EntityState.Detached);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        var productList = result.ToList();
        productList.Should().HaveCount(3);
        productList.Count(p => p.Category!.Name == "Electronics").Should().Be(2);
        productList.Count(p => p.Category!.Name == "Books").Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_CalledMultipleTimes_ReturnsConsistentResults()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Consistent Product",
            Description = "Should return same data",
            Price = 50m,
            Stock = 10,
            CategoryId = _testCategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _context.Entry(product).State = EntityState.Detached;

        // Act
        var result1 = await _sut.GetByIdAsync(productId);
        var result2 = await _sut.GetByIdAsync(productId);
        var result3 = await _sut.GetByIdAsync(productId);

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result3.Should().NotBeNull();
        result1!.Id.Should().Be(result2!.Id).And.Be(result3!.Id);
        result1.Name.Should().Be(result2.Name).And.Be(result3.Name);
        result1.Price.Should().Be(result2.Price).And.Be(result3.Price);
    }

    [Fact]
    public async Task GetAllAsync_WithLargeDataset_ReturnsAllProducts()
    {
        // Arrange - Add 50 products
        var products = Enumerable.Range(1, 50).Select(i => new Product
        {
            Id = Guid.NewGuid(),
            Name = $"Product {i}",
            Description = $"Description for product {i}",
            Price = i * 10m,
            Stock = i * 2,
            CategoryId = _testCategoryId
        }).ToList();

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
        products.ForEach(p => _context.Entry(p).State = EntityState.Detached);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(50);
        result.Should().OnlyContain(p => p.Category != null);
        result.Should().OnlyContain(p => p.Category!.Name == "Test Category");
    }
}


