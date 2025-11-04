using FakeItEasy;
using FluentAssertions;
using Shop.Application.Dtos;
using Shop.Application.Services.Category;
using Shop.Domain.Entities;
using Shop.Infrastructure.Repositories;

namespace Shop.Api.Tests.Services;

public class CategoryServiceTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _categoryRepository = A.Fake<ICategoryRepository>();
        _sut = new CategoryService(_categoryRepository);
    }

    #region GetAllCategoriesAsync Tests

    public class GetAllCategoriesAsync : CategoryServiceTests
    {
        [Fact]
        public async Task ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
       new()
              {
              Id = Guid.NewGuid(),
            Name = "Electronics",
            Description = "Electronic gadgets and devices",
     Products = new List<Product>
           {
    new() { Id = Guid.NewGuid(), Name = "Laptop", Price = 999m, Stock = 10 },
       new() { Id = Guid.NewGuid(), Name = "Phone", Price = 699m, Stock = 20 }
           }
      },
       new()
      {
   Id = Guid.NewGuid(),
Name = "Books",
    Description = "Reading materials",
         Products = new List<Product>
           {
          new() { Id = Guid.NewGuid(), Name = "Novel", Price = 19.99m, Stock = 50 }
      }
    }
 };

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            var categoryList = result.ToList();
            categoryList.Should().HaveCount(2);
            categoryList[0].Name.Should().Be("Electronics");
            categoryList[0].ProductCount.Should().Be(2);
            categoryList[1].Name.Should().Be("Books");
            categoryList[1].ProductCount.Should().Be(1);
        }

        [Fact]
        public async Task WhenNoCategories_ReturnsEmptyList()
        {
            // Arrange
            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(new List<Category>());

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task WithCategoriesWithoutProducts_ReturnsZeroProductCount()
        {
            // Arrange
            var categories = new List<Category>
      {
       new()
       {
         Id = Guid.NewGuid(),
              Name = "Empty Category",
            Description = "No products yet",
            Products = new List<Product>()
           },
      new()
      {
     Id = Guid.NewGuid(),
        Name = "Null Products",
    Description = "Products is null",
        Products = null!
        }
 };

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            var categoryList = result.ToList();
            categoryList.Should().HaveCount(2);
            categoryList[0].ProductCount.Should().Be(0);
            categoryList[1].ProductCount.Should().Be(0);
        }

        [Fact]
        public async Task WithManyProducts_CountsCorrectly()
        {
            // Arrange
            var products = Enumerable.Range(1, 100).Select(i => new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Product {i}",
                Price = i * 10m,
                Stock = i
            }).ToList();

            var categories = new List<Category>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Large Category",
                    Description = "Many products",
                    Products = products
                }
            };

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            result.First().ProductCount.Should().Be(100);
        }

        [Fact]
        public async Task CallsRepositoryOnce()
        {
            // Arrange
            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(new List<Category>());

            // Act
            await _sut.GetAllCategoriesAsync();

            // Assert
            A.CallTo(() => _categoryRepository.GetAllAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task WithNullDescription_ReturnsCategory()
        {
            // Arrange
            var categories = new List<Category>
      {
            new()
                 {
                Id = Guid.NewGuid(),
                Name = "No Description",
                Description = null,
                Products = new List<Product>()
      }
            };

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            result.First().Description.Should().BeNull();
        }
    }

    #endregion

    #region GetCategoryByIdAsync Tests

    public class GetCategoryByIdAsync : CategoryServiceTests
    {
        [Fact]
        public async Task WithValidId_ReturnsCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category
            {
                Id = categoryId,
                Name = "Electronics",
                Description = "Electronic devices",
                Products = new List<Product>
                {
       new() { Id = Guid.NewGuid(), Name = "Laptop", Price = 999m, Stock = 10 }
     }
            };

            A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId)).Returns(category);

            // Act
            var result = await _sut.GetCategoryByIdAsync(categoryId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(categoryId);
            result.Name.Should().Be("Electronics");
            result.Description.Should().Be("Electronic devices");
            result.ProductCount.Should().Be(1);
        }

        [Fact]
        public async Task WithInvalidId_ReturnsNull()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId)).Returns((Category?)null);

            // Act
            var result = await _sut.GetCategoryByIdAsync(categoryId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task WithEmptyGuid_ReturnsNull()
        {
            // Arrange
            A.CallTo(() => _categoryRepository.GetByIdAsync(Guid.Empty)).Returns((Category?)null);

            // Act
            var result = await _sut.GetCategoryByIdAsync(Guid.Empty);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task WithNullProducts_ReturnsZeroCount()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category
            {
                Id = categoryId,
                Name = "Empty Category",
                Description = "No products",
                Products = null!
            };

            A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId)).Returns(category);

            // Act
            var result = await _sut.GetCategoryByIdAsync(categoryId);

            // Assert
            result.Should().NotBeNull();
            result!.ProductCount.Should().Be(0);
        }

        [Fact]
        public async Task WithEmptyProductsList_ReturnsZeroCount()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category
            {
                Id = categoryId,
                Name = "Empty Category",
                Description = "No products",
                Products = new List<Product>()
            };

            A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId)).Returns(category);

            // Act
            var result = await _sut.GetCategoryByIdAsync(categoryId);

            // Assert
            result.Should().NotBeNull();
            result!.ProductCount.Should().Be(0);
        }

        [Fact]
        public async Task CallsRepositoryWithCorrectId()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId)).Returns((Category?)null);

            // Act
            await _sut.GetCategoryByIdAsync(categoryId);

            // Assert
            A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task MapsAllPropertiesCorrectly()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category
            {
                Id = categoryId,
                Name = "Test Category",
                Description = "Test Description",
                Products = Enumerable.Range(1, 5).Select(i => new Product
                {
                    Id = Guid.NewGuid(),
                    Name = $"Product {i}",
                    Price = i * 10m,
                    Stock = i
                }).ToList()
            };

            A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId)).Returns(category);

            // Act
            var result = await _sut.GetCategoryByIdAsync(categoryId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(categoryId);
            result.Name.Should().Be("Test Category");
            result.Description.Should().Be("Test Description");
            result.ProductCount.Should().Be(5);
        }
    }

    #endregion

    #region Edge Cases & Boundary Tests

    public class EdgeCasesAndBoundaryTests : CategoryServiceTests
    {
        [Fact]
        public async Task GetAllCategoriesAsync_WithSpecialCharactersInName_ReturnsCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
            new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Category with 'special' <characters> & symbols! ?? ??",
                    Description = "Contains: ñ, é, ü",
                    Products = new List<Product>()
                }
            };

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            result.First().Name.Should().Contain("??");
            result.First().Description.Should().Contain("ñ");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WithVeryLongDescription_ReturnsCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var longDescription = new string('A', 5000);
            var category = new Category
            {
                Id = categoryId,
                Name = "Category",
                Description = longDescription,
                Products = new List<Product>()
            };

            A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId)).Returns(category);

            // Act
            var result = await _sut.GetCategoryByIdAsync(categoryId);

            // Assert
            result.Should().NotBeNull();
            result!.Description.Should().HaveLength(5000);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_WithMaximumProducts_HandlesCorrectly()
        {
            // Arrange
            var manyProducts = Enumerable.Range(1, 10000).Select(i => new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Product {i}",
                Price = 1m,
                Stock = 1
            }).ToList();

            var categories = new List<Category>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Massive Category",
                    Description = "Lots of products",
                    Products = manyProducts
                }
            };

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            result.First().ProductCount.Should().Be(10000);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_CalledMultipleTimes_CallsRepositoryEachTime()
        {
            // Arrange
            var categories = new List<Category>
 {
            new()
            {
            Id = Guid.NewGuid(),
            Name = "Test",
            Description = "Test",
            Products = new List<Product>()
            }
        };

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            await _sut.GetAllCategoriesAsync();
            await _sut.GetAllCategoriesAsync();
            await _sut.GetAllCategoriesAsync();

            // Assert
            A.CallTo(() => _categoryRepository.GetAllAsync()).MustHaveHappened(3, Times.Exactly);
        }
    }

    #endregion

    #region Mapping Tests

    public class MappingTests : CategoryServiceTests
    {
        [Fact]
        public async Task GetAllCategoriesAsync_MapsAllFieldsCorrectly()
        {
            // Arrange
            var categoryId1 = Guid.NewGuid();
            var categoryId2 = Guid.NewGuid();

            var categories = new List<Category>
{
            new()
            {
                Id = categoryId1,
                Name = "Category 1",
                Description = "Description 1",
                Products = new List<Product> { new() { Id = Guid.NewGuid() } }
                },
                    new()
                    {
                    Id = categoryId2,
                    Name = "Category 2",
                    Description = null,
                    Products = null!
                    }
            };

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            var categoryList = result.ToList();
            categoryList[0].Id.Should().Be(categoryId1);
            categoryList[0].Name.Should().Be("Category 1");
            categoryList[0].Description.Should().Be("Description 1");
            categoryList[0].ProductCount.Should().Be(1);

            categoryList[1].Id.Should().Be(categoryId2);
            categoryList[1].Name.Should().Be("Category 2");
            categoryList[1].Description.Should().BeNull();
            categoryList[1].ProductCount.Should().Be(0);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_PreservesAllProperties()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId,
                Name = "Preserved Category",
                Description = "All properties preserved",
                Products = new List<Product>
                {
                    new() { Id = productId1, Name = "Product 1" },
                    new() { Id = productId2, Name = "Product 2" }
                }
            };

            A.CallTo(() => _categoryRepository.GetByIdAsync(categoryId)).Returns(category);

            // Act
            var result = await _sut.GetCategoryByIdAsync(categoryId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(categoryId);
            result.Name.Should().Be("Preserved Category");
            result.Description.Should().Be("All properties preserved");
            result.ProductCount.Should().Be(2);
        }
    }

    #endregion

    #region Performance & Collection Tests

    public class PerformanceAndCollectionTests : CategoryServiceTests
    {
        [Fact]
        public async Task GetAllCategoriesAsync_WithManyCategories_ReturnsAll()
        {
            // Arrange
            var categories = Enumerable.Range(1, 100).Select(i => new Category
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}",
                Description = $"Description {i}",
                Products = Enumerable.Range(1, i).Select(j => new Product
                {
                    Id = Guid.NewGuid(),
                    Name = $"Product {j}",
                    Price = j,
                    Stock = j
                }).ToList()
            }).ToList();

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            var categoryList = result.ToList();
            categoryList.Should().HaveCount(100);
            categoryList[0].ProductCount.Should().Be(1);
            categoryList[99].ProductCount.Should().Be(100);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_PreservesOrder()
        {
            // Arrange
            var categories = new List<Category>
            {
                new() { Id = Guid.NewGuid(), Name = "Zebra", Description = "Last", Products = new List<Product>() },
                new() { Id = Guid.NewGuid(), Name = "Alpha", Description = "First", Products = new List<Product>() },
                new() { Id = Guid.NewGuid(), Name = "Beta", Description = "Middle", Products = new List<Product>() }
            };

            A.CallTo(() => _categoryRepository.GetAllAsync()).Returns(categories);

            // Act
            var result = await _sut.GetAllCategoriesAsync();

            // Assert
            var categoryList = result.ToList();
            categoryList[0].Name.Should().Be("Zebra");
            categoryList[1].Name.Should().Be("Alpha");
            categoryList[2].Name.Should().Be("Beta");
        }
    }

    #endregion
}
