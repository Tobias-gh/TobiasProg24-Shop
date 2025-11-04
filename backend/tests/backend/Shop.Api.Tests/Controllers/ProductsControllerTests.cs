using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Controllers;
using Shop.Application.Dtos;
using Shop.Application.Services.Product;

namespace Shop.Api.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly IProductService _productService;
    private readonly ProductsController _sut;

    public ProductsControllerTests()
    {
        _productService = A.Fake<IProductService>();
        _sut = new ProductsController(_productService);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new(Guid.NewGuid(), "Laptop", "Gaming laptop", 1299.99m, 10, Guid.NewGuid(), "Electronics"),
            new(Guid.NewGuid(), "Mouse", "Wireless mouse", 29.99m, 50, Guid.NewGuid(), "Electronics")
        };

        A.CallTo(() => _productService.GetAllProductsAsync()).Returns(products);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithEmptyList_WhenNoProducts()
    {
        // Arrange
        A.CallTo(() => _productService.GetAllProductsAsync()).Returns(new List<ProductDto>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var products = okResult!.Value as IEnumerable<ProductDto>;
        products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetbyId_ReturnsOkResult_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new ProductDto(productId, "Laptop", "Gaming laptop", 1299.99m, 10, Guid.NewGuid(), "Electronics");

        A.CallTo(() => _productService.GetProductByIdAsync(productId)).Returns(product);

        // Act
        var result = await _sut.GetbyId(productId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async Task GetbyId_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => _productService.GetProductByIdAsync(productId)).Returns((ProductDto?)null);

        // Act
        var result = await _sut.GetbyId(productId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetbyId_ReturnsNotFoundWithMessage_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => _productService.GetProductByIdAsync(productId)).Returns((ProductDto?)null);

        // Act
        var result = await _sut.GetbyId(productId);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        var message = notFoundResult!.Value!.GetType().GetProperty("message")!.GetValue(notFoundResult.Value);
        message.Should().Be($"product with ID {productId} not found");
    }

    [Fact]
    public async Task GetAll_CallsServiceOnce()
    {
        // Arrange
        A.CallTo(() => _productService.GetAllProductsAsync()).Returns(new List<ProductDto>());

        // Act
        await _sut.GetAll();

        // Assert
        A.CallTo(() => _productService.GetAllProductsAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetbyId_CallsServiceWithCorrectId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => _productService.GetProductByIdAsync(productId)).Returns((ProductDto?)null);

        // Act
        await _sut.GetbyId(productId);

        // Assert
        A.CallTo(() => _productService.GetProductByIdAsync(productId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetbyId_WithValidId_ReturnsStatusCode200()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new ProductDto(productId, "Product", null, 50m, 10, Guid.NewGuid(), "Category");

        A.CallTo(() => _productService.GetProductByIdAsync(productId)).Returns(product);

        // Act
        var result = await _sut.GetbyId(productId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetbyId_WithInvalidId_ReturnsStatusCode404()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => _productService.GetProductByIdAsync(productId)).Returns((ProductDto?)null);

        // Act
        var result = await _sut.GetbyId(productId);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetAll_WhenServiceThrows_PropagatesException()
    {
        // Arrange
        A.CallTo(() => _productService.GetAllProductsAsync()).Throws<InvalidOperationException>();

        // Act
        Func<Task> act = async () => await _sut.GetAll();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetbyId_WhenServiceThrows_PropagatesException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        A.CallTo(() => _productService.GetProductByIdAsync(productId)).Throws<InvalidOperationException>();

        // Act
        Func<Task> act = async () => await _sut.GetbyId(productId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetbyId_WithEmptyGuid_ReturnsNotFound()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        A.CallTo(() => _productService.GetProductByIdAsync(emptyGuid)).Returns((ProductDto?)null);

        // Act
        var result = await _sut.GetbyId(emptyGuid);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
