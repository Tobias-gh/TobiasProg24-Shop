using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;

namespace Shop.Integration.Tests.Repositories;

public class CategoryRepositoryTests : IDisposable
{
    private readonly ShopDbContext _context;
    private readonly CategoryRepository _sut;

    public CategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ShopDbContext>()
     .UseInMemoryDatabase($"test_db_{Guid.NewGuid()}")
              .Options;
        _context = new ShopDbContext(options);
        _sut = new CategoryRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCategoryWithProducts()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Electronics",
            Description = "Electronic devices"
        };

        var products = new List<Product>
        {
            new()
       {
   Id = Guid.NewGuid(),
          Name = "Laptop",
          Price = 999m,
         Stock = 10,
                CategoryId = categoryId
      },
            new()
            {
                Id = Guid.NewGuid(),
        Name = "Mouse",
       Price = 29.99m,
    Stock = 50,
    CategoryId = categoryId
            }
    };

        _context.Categories.Add(category);
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
        _context.Entry(category).State = EntityState.Detached;
        products.ForEach(p => _context.Entry(p).State = EntityState.Detached);

        // Act
        var result = await _sut.GetByIdAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(categoryId);
        result.Name.Should().Be("Electronics");
        result.Description.Should().Be("Electronic devices");
        result.Products.Should().HaveCount(2);
        result.Products.Should().Contain(p => p.Name == "Laptop");
        result.Products.Should().Contain(p => p.Name == "Mouse");
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleCategories_ReturnsAllWithProducts()
    {
        // Arrange
        var category1 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            Description = "Electronic devices"
        };

        var category2 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Books",
            Description = "Reading materials"
        };

        var products = new List<Product>
        {
   new()
            {
                Id = Guid.NewGuid(),
    Name = "Laptop",
          Price = 999m,
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

        _context.Categories.AddRange(category1, category2);
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
        _context.Entry(category1).State = EntityState.Detached;
        _context.Entry(category2).State = EntityState.Detached;
        products.ForEach(p => _context.Entry(p).State = EntityState.Detached);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        var categoryList = result.ToList();
        categoryList.Should().HaveCount(2);
        categoryList.Should().Contain(c => c.Name == "Electronics");
        categoryList.Should().Contain(c => c.Name == "Books");
        categoryList.First(c => c.Name == "Electronics").Products.Should().HaveCount(1);
        categoryList.First(c => c.Name == "Books").Products.Should().HaveCount(1);
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
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WithNullDescription_ReturnsCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "No Description Category",
            Description = null
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        _context.Entry(category).State = EntityState.Detached;

        // Act
        var result = await _sut.GetByIdAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().BeNull();
        result.Name.Should().Be("No Description Category");
    }

    [Fact]
    public async Task GetByIdAsync_WithNoProducts_ReturnsCategoryWithEmptyProductsList()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Empty Category",
            Description = "No products yet"
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        _context.Entry(category).State = EntityState.Detached;

        // Act
        var result = await _sut.GetByIdAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result!.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WithManyProductsPerCategory_LoadsAllProducts()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Tech",
            Description = "Technology products"
        };

        var products = Enumerable.Range(1, 20).Select(i => new Product
        {
            Id = Guid.NewGuid(),
            Name = $"Product {i}",
            Price = i * 10m,
            Stock = i,
            CategoryId = categoryId
        }).ToList();

        _context.Categories.Add(category);
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
        _context.Entry(category).State = EntityState.Detached;
        products.ForEach(p => _context.Entry(p).State = EntityState.Detached);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        var categoryList = result.ToList();
        categoryList.Should().HaveCount(1);
        categoryList.First().Products.Should().HaveCount(20);
    }

    [Fact]
    public async Task GetByIdAsync_CalledMultipleTimes_ReturnsConsistentResults()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Consistent Category",
            Description = "Should be consistent"
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        _context.Entry(category).State = EntityState.Detached;

        // Act
        var result1 = await _sut.GetByIdAsync(categoryId);
        var result2 = await _sut.GetByIdAsync(categoryId);
        var result3 = await _sut.GetByIdAsync(categoryId);

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result3.Should().NotBeNull();
        result1!.Id.Should().Be(result2!.Id).And.Be(result3!.Id);
        result1.Name.Should().Be(result2.Name).And.Be(result3.Name);
    }
}
