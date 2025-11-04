using FluentAssertions;
using Shop.Domain.Entities;

namespace Shop.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Product_CanBeCreated()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Price = 10m,
            Stock = 5,
            CategoryId = Guid.NewGuid()
        };

        product.Should().NotBeNull();
    }
}
