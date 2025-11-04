using FluentAssertions;
using Shop.Application.Dtos;

namespace Shop.Application.Tests.Dtos;

public class CartDtoTests
{
    #region Record Immutability Tests

    public class RecordImmutabilityTests
    {
        [Fact]
        public void CartDto_IsRecord_CannotModifyAfterCreation()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var sessionId = "session-123";
            var items = new List<CartItemDto>();

            // Act
            var cart = new CartDto(
               cartId,
                      sessionId,
               items,
         0,
                     0m,
                    DateTime.UtcNow,
                  DateTime.UtcNow
           );

            // Assert
            cart.Id.Should().Be(cartId);
            cart.SessionId.Should().Be(sessionId);
            // Records are immutable - cannot reassign properties
        }

        [Fact]
        public void CartItemDto_IsRecord_CannotModifyAfterCreation()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Act
            var item = new CartItemDto(
       itemId,
     productId,
 "Product Name",
        "Description",
          99.99m,
           2,
                199.98m,
       10,
     DateTime.UtcNow
  );

            // Assert
            item.Id.Should().Be(itemId);
            item.Quantity.Should().Be(2);
        }

        [Fact]
        public void AddToCartRequest_IsRecord_StoresValuesCorrectly()
        {
            // Arrange & Act
            var productId = Guid.NewGuid();
            var request = new AddToCartRequest(productId, 5);

            // Assert
            request.ProductId.Should().Be(productId);
            request.Quantity.Should().Be(5);
        }
    }

    #endregion

    #region Value Equality Tests

    public class ValueEqualityTests
    {
        [Fact]
        public void CartDto_WithSameValues_AreEqual()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var sessionId = "session-123";
            var createdAt = DateTime.UtcNow;
            var updatedAt = DateTime.UtcNow;
            var items = new List<CartItemDto>();

            // Act
            var cart1 = new CartDto(cartId, sessionId, items, 0, 0m, createdAt, updatedAt);
            var cart2 = new CartDto(cartId, sessionId, items, 0, 0m, createdAt, updatedAt);

            // Assert
            cart1.Should().Be(cart2);
            cart1.GetHashCode().Should().Be(cart2.GetHashCode());
        }

        [Fact]
        public void CartDto_WithDifferentValues_AreNotEqual()
        {
            // Arrange
            var items = new List<CartItemDto>();
            var createdAt = DateTime.UtcNow;
            var updatedAt = DateTime.UtcNow;

            // Act
            var cart1 = new CartDto(Guid.NewGuid(), "session-1", items, 0, 0m, createdAt, updatedAt);
            var cart2 = new CartDto(Guid.NewGuid(), "session-2", items, 0, 0m, createdAt, updatedAt);

            // Assert
            cart1.Should().NotBe(cart2);
        }

        [Fact]
        public void CartItemDto_WithSameValues_AreEqual()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var addedAt = DateTime.UtcNow;

            // Act
            var item1 = new CartItemDto(itemId, productId, "Product", "Desc", 99m, 2, 198m, 10, addedAt);
            var item2 = new CartItemDto(itemId, productId, "Product", "Desc", 99m, 2, 198m, 10, addedAt);

            // Assert
            item1.Should().Be(item2);
            item1.GetHashCode().Should().Be(item2.GetHashCode());
        }

        [Fact]
        public void AddToCartRequest_WithSameValues_AreEqual()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var request1 = new AddToCartRequest(productId, 5);
            var request2 = new AddToCartRequest(productId, 5);

            // Assert
            request1.Should().Be(request2);
        }
    }

    #endregion

    #region With Expression Tests

    public class WithExpressionTests
    {
        [Fact]
        public void CartDto_WithExpression_CreatesNewInstanceWithModifiedProperty()
        {
            // Arrange
            var originalCart = new CartDto(
                Guid.NewGuid(),
 "session-123",
                new List<CartItemDto>(),
     0,
       0m,
                DateTime.UtcNow,
                DateTime.UtcNow
            );

            // Act
            var modifiedCart = originalCart with { TotalItems = 5, TotalPrice = 250m };

            // Assert
            modifiedCart.TotalItems.Should().Be(5);
            modifiedCart.TotalPrice.Should().Be(250m);
            modifiedCart.Id.Should().Be(originalCart.Id);
            modifiedCart.SessionId.Should().Be(originalCart.SessionId);
            originalCart.TotalItems.Should().Be(0); // Original unchanged
        }

        [Fact]
        public void CartItemDto_WithExpression_UpdatesQuantityAndSubtotal()
        {
            // Arrange
            var originalItem = new CartItemDto(
     Guid.NewGuid(),
           Guid.NewGuid(),
            "Product",
             "Description",
       50m,
         2,
         100m,
      10,
      DateTime.UtcNow
  );

            // Act
            var modifiedItem = originalItem with { Quantity = 5, Subtotal = 250m };

            // Assert
            modifiedItem.Quantity.Should().Be(5);
            modifiedItem.Subtotal.Should().Be(250m);
            originalItem.Quantity.Should().Be(2); // Original unchanged
        }
    }

    #endregion

    #region CartSummaryDto Tests

    public class CartSummaryDtoTests
    {
        [Fact]
        public void CartSummaryDto_WithZeroValues_IsValid()
        {
            // Act
            var summary = new CartSummaryDto(0, 0m);

            // Assert
            summary.TotalItems.Should().Be(0);
            summary.TotalPrice.Should().Be(0m);
        }

        [Fact]
        public void CartSummaryDto_WithPositiveValues_IsValid()
        {
            // Act
            var summary = new CartSummaryDto(10, 499.99m);

            // Assert
            summary.TotalItems.Should().Be(10);
            summary.TotalPrice.Should().Be(499.99m);
        }

        [Fact]
        public void CartSummaryDto_WithSameValues_AreEqual()
        {
            // Arrange & Act
            var summary1 = new CartSummaryDto(5, 250m);
            var summary2 = new CartSummaryDto(5, 250m);

            // Assert
            summary1.Should().Be(summary2);
        }
    }

    #endregion

    #region UpdateCartItemRequest Tests

    public class UpdateCartItemRequestTests
    {
        [Fact]
        public void UpdateCartItemRequest_StoresQuantityCorrectly()
        {
            // Act
            var request = new UpdateCartItemRequest(10);

            // Assert
            request.Quantity.Should().Be(10);
        }

        [Fact]
        public void UpdateCartItemRequest_WithSameValues_AreEqual()
        {
            // Arrange & Act
            var request1 = new UpdateCartItemRequest(5);
            var request2 = new UpdateCartItemRequest(5);

            // Assert
            request1.Should().Be(request2);
        }
    }

    #endregion

    #region Edge Cases & Boundary Tests

    public class EdgeCasesAndBoundaryTests
    {
        [Fact]
        public void CartDto_WithEmptySessionId_IsValid()
        {
            // Act
            var cart = new CartDto(
             Guid.NewGuid(),
                        "",
              new List<CartItemDto>(),
             0,
            0m,
         DateTime.UtcNow,
             DateTime.UtcNow
                    );

            // Assert
            cart.SessionId.Should().Be("");
        }

        [Fact]
        public void CartItemDto_WithNullDescription_IsValid()
        {
            // Act
            var item = new CartItemDto(
   Guid.NewGuid(),
     Guid.NewGuid(),
          "Product",
             null,
   99m,
         1,
   99m,
     10,
   DateTime.UtcNow
            );

            // Assert
            item.ProductDescription.Should().BeNull();
        }

        [Fact]
        public void CartItemDto_WithZeroPrice_IsValid()
        {
            // Act
            var item = new CartItemDto(
          Guid.NewGuid(),
     Guid.NewGuid(),
            "Free Product",
 "Free sample",
        0m,
    1,
     0m,
        100,
   DateTime.UtcNow
     );

            // Assert
            item.ProductPrice.Should().Be(0m);
            item.Subtotal.Should().Be(0m);
        }

        [Fact]
        public void CartItemDto_WithLargeQuantity_IsValid()
        {
            // Act
            var item = new CartItemDto(
            Guid.NewGuid(),
  Guid.NewGuid(),
      "Product",
      "Desc",
   1m,
        10000,
     10000m,
     20000,
         DateTime.UtcNow
    );

            // Assert
            item.Quantity.Should().Be(10000);
            item.Subtotal.Should().Be(10000m);
        }

        [Fact]
        public void CartDto_WithManyItems_IsValid()
        {
            // Arrange
            var items = Enumerable.Range(1, 100).Select(i => new CartItemDto(
             Guid.NewGuid(),
                    Guid.NewGuid(),
                 $"Product {i}",
                   "Description",
             10m,
              1,
                  10m,
                  10,
                    DateTime.UtcNow
            )).ToList();

            // Act
            var cart = new CartDto(
          Guid.NewGuid(),
            "session-123",
                 items,
               100,
                  1000m,
                 DateTime.UtcNow,
                      DateTime.UtcNow
        );

            // Assert
            cart.Items.Should().HaveCount(100);
            cart.TotalItems.Should().Be(100);
        }

        [Fact]
        public void AddToCartRequest_WithGuidEmpty_IsValid()
        {
            // Act
            var request = new AddToCartRequest(Guid.Empty, 1);

            // Assert
            request.ProductId.Should().Be(Guid.Empty);
        }
    }

    #endregion

    #region Deconstruction Tests

    public class DeconstructionTests
    {
        [Fact]
        public void CartSummaryDto_CanBeDeconstructed()
        {
            // Arrange
            var summary = new CartSummaryDto(10, 500m);

            // Act
            var (totalItems, totalPrice) = summary;

            // Assert
            totalItems.Should().Be(10);
            totalPrice.Should().Be(500m);
        }

        [Fact]
        public void AddToCartRequest_CanBeDeconstructed()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new AddToCartRequest(productId, 5);

            // Act
            var (productIdResult, quantity) = request;

            // Assert
            productIdResult.Should().Be(productId);
            quantity.Should().Be(5);
        }
    }

    #endregion

    #region ToString Tests

    public class ToStringTests
    {
        [Fact]
        public void CartDto_ToString_ContainsKeyInformation()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cart = new CartDto(
  cartId,
      "session-123",
                new List<CartItemDto>(),
          0,
          0m,
    DateTime.UtcNow,
          DateTime.UtcNow
      );

            // Act
            var toString = cart.ToString();

            // Assert
            toString.Should().Contain(cartId.ToString());
            toString.Should().Contain("session-123");
        }

        [Fact]
        public void AddToCartRequest_ToString_ContainsValues()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new AddToCartRequest(productId, 5);

            // Act
            var toString = request.ToString();

            // Assert
            toString.Should().Contain(productId.ToString());
            toString.Should().Contain("5");
        }
    }

    #endregion
}
