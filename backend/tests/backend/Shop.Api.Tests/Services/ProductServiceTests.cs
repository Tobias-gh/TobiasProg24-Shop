using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Shop.Application.Services.Product;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Api.Tests.Services;

public class ProductServiceTests
{
    private readonly IProductRepository _productRepository;
    //private readonly ICategoryRepository _categoryRepository;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepository = A.Fake<IProductRepository>();
        //_categoryRepository = A.Fake<ICategoryRepository>();

        _sut = new ProductService(_productRepository);
    }

    #region GetAllProductsAsync Tests

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        //Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            Description = "Electronic gadgets and devices"
        };

        var product = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Smartphone",
                Description = "Latest model smartphone",
                Price = 699.99m,
                CategoryId = category.Id,
                Category = category,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Mouse",
                Description = "Wireless mouse",
                Price = 99.99m,
                CategoryId = category.Id,
                Category = category,
            }

        };

        A.CallTo(() => _productRepository.GetAllAsync()).Returns(product);

        //Act
        var result = await _sut.GetAllProductsAsync();



        //Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Smartphone");
        result.Should().Contain(p => p.Name == "Mouse");


    }
    [Fact]
    public async Task GetAllProductsAsync_WhenNoProducts_ReturnsEmptyList()
    {
        //arrage
        A.CallTo(() => _productRepository.GetAllAsync()).Returns(new List<Product>());

        //Act
        var result = await _sut.GetAllProductsAsync();

        //assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllProductsAsync_WithProductsWithoutCategory_ReturnsCategorized()
    {
        //arrange
        var products = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Desk Lamp",
                Description = "LED desk lamp",
                Price = 49.99m,
                CategoryId = Guid.NewGuid(),
                Category = null
           }
        };

        A.CallTo(() => _productRepository.GetAllAsync()).Returns(products);

        //act
        var result = await _sut.GetAllProductsAsync();

        //assert
        result.Should().HaveCount(1);
        result.First().CategoryName.Should().Be("Uncategorized");
    }


    #endregion

    #region GetProductByIdAsync Tests
    [Fact]
    public async Task GetProductByIdAsync_ValidId_ReturnsProduct()
    {
        //arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Laptop",
            Description = "High performance laptop",
            Price = 1299.99m,
            Stock = 10,
            CategoryId = categoryId,
            Category = new Category
            {
                Id = categoryId,
                Name = "Computers",
                Description = "All kinds of computers"
            }
        };

        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

        //act 
        var result = await _sut.GetProductByIdAsync(productId);

        //Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Laptop");
        result.Price.Should().Be(1299.99m);
        result.CategoryName.Should().Be("Computers");
        result.Stock.Should().Be(10);
    }

    [Fact]
    public async Task GetProductByIdAsync_InvalidId_ReturnsNull()
    {
        //Arrage
        var productId = Guid.NewGuid();
        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns((Product?)null);

        //Act
        var result = await _sut.GetProductByIdAsync(productId);

        //Assert
        result.Should().BeNull();
    }

    #endregion

    #region Mapping Tests
    [Fact]
    public async Task GetProductByIdAsync_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 999.99m,
            Stock = 25,
            CategoryId = categoryId,
            Category = new Category
            {
                Id = categoryId,
                Name = "Test Category",
                Description = "Category Description"
            }
        };

        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
        result.Description.Should().Be("Test Description");
        result.Price.Should().Be(999.99m);
        result.Stock.Should().Be(25);
        result.CategoryId.Should().Be(categoryId);
        result.CategoryName.Should().Be("Test Category");
    }

    [Fact]
    public async Task GetAllProductsAsync_MapsMultipleProductsCorrectly()
    {
        // Arrange
        var category1 = new Category { Id = Guid.NewGuid(), Name = "Electronics", Description = "Gadgets" };
        var category2 = new Category { Id = Guid.NewGuid(), Name = "Books", Description = "Reading" };

        var products = new List<Product>
    {
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            Price = 1299.99m,
            Stock = 10,
            CategoryId = category1.Id,
            Category = category1
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Novel",
            Price = 19.99m,
            Stock = 50,
            CategoryId = category2.Id,
            Category = category2
        }
    };

        A.CallTo(() => _productRepository.GetAllAsync()).Returns(products);

        // Act
        var result = await _sut.GetAllProductsAsync();

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(2);
        resultList[0].CategoryName.Should().Be("Electronics");
        resultList[1].CategoryName.Should().Be("Books");
    }

    #endregion

    #region Boundary Value Tests
    [Fact]
    public async Task GetProductByIdAsync_WithZeroPrice_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Free Product",
            Price = 0m,  // Boundary: zero price
            Stock = 10,
            CategoryId = Guid.NewGuid(),
            Category = new Category { Id = Guid.NewGuid(), Name = "Freebies" }
        };

        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Price.Should().Be(0m);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithZeroStock_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Out of Stock",
            Price = 99.99m,
            Stock = 0,  // Boundary: zero stock
            CategoryId = Guid.NewGuid(),
            Category = new Category { Id = Guid.NewGuid(), Name = "Electronics" }
        };

        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Stock.Should().Be(0);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNullDescription_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Minimal Product",
            Description = null,  // Nullable field
            Price = 99.99m,
            Stock = 10,
            CategoryId = Guid.NewGuid(),
            Category = new Category { Id = Guid.NewGuid(), Name = "Electronics" }
        };

        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().BeNull();
    }
    #endregion 

    #region Repository Interaction Tests
    [Fact]
    public async Task GetAllProductsAsync_CallsRepositoryOnce()
    {
        // Arrange
        A.CallTo(() => _productRepository.GetAllAsync()).Returns(new List<Product>());

        // Act
        await _sut.GetAllProductsAsync();

        // Assert
        A.CallTo(() => _productRepository.GetAllAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetProductByIdAsync_CallsRepositoryWithCorrectId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns((Product?)null);

        // Act
        await _sut.GetProductByIdAsync(productId);

        // Assert
        A.CallTo(() => _productRepository.GetByIdAsync(productId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetAllProductsAsync_CallsRepositoryMultipleTimes_ReturnsSameData()
    {
        // Arrange
        var products = new List<Product>
    {
        new() { Id = Guid.NewGuid(), Name = "Product1", Price = 10m, Stock = 5, CategoryId = Guid.NewGuid() }
    };
        A.CallTo(() => _productRepository.GetAllAsync()).Returns(products);

        // Act
        var result1 = await _sut.GetAllProductsAsync();
        var result2 = await _sut.GetAllProductsAsync();

        // Assert
        result1.Should().HaveCount(1);
        result2.Should().HaveCount(1);
        A.CallTo(() => _productRepository.GetAllAsync()).MustHaveHappened(2, Times.Exactly);
    }

    #endregion
    #region Collection Handling Tests

    [Fact]
    public async Task GetAllProductsAsync_WithLargeProductList_ReturnsAllProducts()
    {
        // Arrange
        var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
        var products = Enumerable.Range(1, 100).Select(i => new Product
        {
            Id = Guid.NewGuid(),
            Name = $"Product {i}",
            Price = i * 10m,
            Stock = i,
            CategoryId = category.Id,
            Category = category
        }).ToList();

        A.CallTo(() => _productRepository.GetAllAsync()).Returns(products);

        // Act
        var result = await _sut.GetAllProductsAsync();

        // Assert
        result.Should().HaveCount(100);
    }

    [Fact]
    public async Task GetAllProductsAsync_WithMixedCategories_ReturnsAllProductsWithCorrectCategories()
    {
        // Arrange
        var electronics = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
        var books = new Category { Id = Guid.NewGuid(), Name = "Books" };
        var clothing = new Category { Id = Guid.NewGuid(), Name = "Clothing" };

        var products = new List<Product>
    {
        new() { Id = Guid.NewGuid(), Name = "Laptop", Price = 999m, Stock = 5, CategoryId = electronics.Id, Category = electronics },
        new() { Id = Guid.NewGuid(), Name = "Novel", Price = 19m, Stock = 50, CategoryId = books.Id, Category = books },
        new() { Id = Guid.NewGuid(), Name = "T-Shirt", Price = 29m, Stock = 100, CategoryId = clothing.Id, Category = clothing },
        new() { Id = Guid.NewGuid(), Name = "Phone", Price = 699m, Stock = 10, CategoryId = electronics.Id, Category = electronics }
    };

        A.CallTo(() => _productRepository.GetAllAsync()).Returns(products);

        // Act
        var result = await _sut.GetAllProductsAsync();

        // Assert
        result.Should().HaveCount(4);
        result.Where(p => p.CategoryName == "Electronics").Should().HaveCount(2);
        result.Where(p => p.CategoryName == "Books").Should().HaveCount(1);
        result.Where(p => p.CategoryName == "Clothing").Should().HaveCount(1);
    }

    #endregion

    #region Data Validation Tests

    [Fact]
    public async Task GetProductByIdAsync_WithSpecialCharactersInName_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Product with 'special' <characters> & symbols!",
            Description = "Contains: ñ, é, ü, 中文, emoji 🔥",
            Price = 99.99m,
            Stock = 10,
            CategoryId = Guid.NewGuid(),
            Category = new Category { Id = Guid.NewGuid(), Name = "Test" }
        };

        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Product with 'special' <characters> & symbols!");
        result.Description.Should().Contain("中文");
    }

    [Fact]
    public async Task GetProductByIdAsync_WithVeryLongDescription_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var longDescription = new string('A', 1000); // 1000 character description

        var product = new Product
        {
            Id = productId,
            Name = "Product",
            Description = longDescription,
            Price = 99.99m,
            Stock = 10,
            CategoryId = Guid.NewGuid(),
            Category = new Category { Id = Guid.NewGuid(), Name = "Test" }
        };

        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().HaveLength(1000);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithMaxDecimalPrice_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Expensive Product",
            Price = 999999999.99m, // Max realistic price
            Stock = 1,
            CategoryId = Guid.NewGuid(),
            Category = new Category { Id = Guid.NewGuid(), Name = "Luxury" }
        };

        A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(product);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Price.Should().Be(999999999.99m);
    }

    #endregion
}
