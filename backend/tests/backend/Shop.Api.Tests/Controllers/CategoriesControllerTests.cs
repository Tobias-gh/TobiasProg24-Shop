using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Controllers;
using Shop.Application.Dtos;
using Shop.Application.Services.Category;

namespace Shop.Api.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly ICategoryService _categoryService;
    private readonly CategoriesController _sut;

    public CategoriesControllerTests()
    {
        _categoryService = A.Fake<ICategoryService>();
        _sut = new CategoriesController(_categoryService);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithCategories()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
            new(Guid.NewGuid(), "Electronics", "Electronic devices", 10),
            new(Guid.NewGuid(), "Books", "Reading materials", 25)
        };

        A.CallTo(() => _categoryService.GetAllCategoriesAsync()).Returns(categories);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(categories);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithEmptyList_WhenNoCategories()
    {
        // Arrange
        A.CallTo(() => _categoryService.GetAllCategoriesAsync()).Returns(new List<CategoryDto>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var categories = okResult!.Value as IEnumerable<CategoryDto>;
        categories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_CallsServiceOnce()
    {
        // Arrange
        A.CallTo(() => _categoryService.GetAllCategoriesAsync()).Returns(new List<CategoryDto>());

        // Act
        await _sut.GetAll();

        // Assert
        A.CallTo(() => _categoryService.GetAllCategoriesAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithMultipleCategories()
    {
        // Arrange
        var categories = Enumerable.Range(1, 20).Select(i => new CategoryDto(
         Guid.NewGuid(),
    $"Category {i}",
            $"Description {i}",
    i * 5
      )).ToList();

        A.CallTo(() => _categoryService.GetAllCategoriesAsync()).Returns(categories);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedCategories = okResult!.Value as IEnumerable<CategoryDto>;
        returnedCategories.Should().HaveCount(20);
    }

    [Fact]
    public async Task GetAll_WhenServiceThrows_PropagatesException()
    {
        // Arrange
        A.CallTo(() => _categoryService.GetAllCategoriesAsync()).Throws<InvalidOperationException>();

        // Act
        Func<Task> act = async () => await _sut.GetAll();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new CategoryDto(categoryId, "Electronics", "Electronic devices", 15);

        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).Returns(category);

        // Act
        var result = await _sut.GetById(categoryId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(category);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).Returns((CategoryDto?)null);

        // Act
        var result = await _sut.GetById(categoryId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetById_ReturnsNotFoundWithMessage_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).Returns((CategoryDto?)null);

        // Act
        var result = await _sut.GetById(categoryId);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        var message = notFoundResult!.Value!.GetType().GetProperty("message")!.GetValue(notFoundResult.Value);
        message.Should().Be($"Category with ID {categoryId} not found");
    }

    [Fact]
    public async Task GetById_CallsServiceWithCorrectId()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).Returns((CategoryDto?)null);

        // Act
        await _sut.GetById(categoryId);

        // Assert
        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetById_WithEmptyGuid_ReturnsNotFound()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        A.CallTo(() => _categoryService.GetCategoryByIdAsync(emptyGuid)).Returns((CategoryDto?)null);

        // Act
        var result = await _sut.GetById(emptyGuid);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsStatusCode200()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new CategoryDto(categoryId, "Category", "Description", 10);

        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).Returns(category);

        // Act
        var result = await _sut.GetById(categoryId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsStatusCode404()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).Returns((CategoryDto?)null);

        // Act
        var result = await _sut.GetById(categoryId);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetById_WhenServiceThrows_PropagatesException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).Throws<InvalidOperationException>();

        // Act
        Func<Task> act = async () => await _sut.GetById(categoryId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetById_ReturnsCategoryWithCorrectProperties()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new CategoryDto(categoryId, "Books", "Reading materials", 100);

        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).Returns(category);

        // Act
        var result = await _sut.GetById(categoryId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        var returnedCategory = okResult!.Value as CategoryDto;
        returnedCategory!.Id.Should().Be(categoryId);
        returnedCategory.Name.Should().Be("Books");
        returnedCategory.ProductCount.Should().Be(100);
    }

    [Fact]
    public async Task GetById_CalledMultipleTimesWithSameId_CallsServiceMultipleTimes()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new CategoryDto(categoryId, "Category", "Desc", 10);

        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).Returns(category);

        // Act
        await _sut.GetById(categoryId);
        await _sut.GetById(categoryId);
        await _sut.GetById(categoryId);

        // Assert
        A.CallTo(() => _categoryService.GetCategoryByIdAsync(categoryId)).MustHaveHappened(3, Times.Exactly);
    }

    [Fact]
    public async Task GetAll_ReturnsCategories_WithDifferentProductCounts()
    {
        // Arrange
        var categories = new List<CategoryDto>
{
       new(Guid.NewGuid(), "Electronics", "Desc", 50),
            new(Guid.NewGuid(), "Books", "Desc", 0),
            new(Guid.NewGuid(), "Clothing", "Desc", 150)
   };

        A.CallTo(() => _categoryService.GetAllCategoriesAsync()).Returns(categories);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var okResult = result.Result as OkObjectResult;
        var returnedCategories = okResult!.Value as IEnumerable<CategoryDto>;
        returnedCategories.Should().HaveCount(3);
        returnedCategories!.Select(c => c.ProductCount).Should().Contain(new[] { 50, 0, 150 });
    }

    [Fact]
    public async Task GetAll_CalledMultipleTimes_CallsServiceEachTime()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
       new(Guid.NewGuid(), "Test", "Test", 1)
        };

        A.CallTo(() => _categoryService.GetAllCategoriesAsync()).Returns(categories);

        // Act
        await _sut.GetAll();
        await _sut.GetAll();
        await _sut.GetAll();

        // Assert
        A.CallTo(() => _categoryService.GetAllCategoriesAsync()).MustHaveHappened(3, Times.Exactly);
    }

    #endregion
}
