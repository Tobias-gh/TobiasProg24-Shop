using FluentAssertions;
using Shop.Application.Dtos;

namespace Shop.Application.Tests.Dtos;

public class ProductDtoTests
{
    #region Record Immutability Tests

    public class RecordImmutabilityTests
    {
        [Fact]
        public void ProductDto_IsRecord_CannotModifyAfterCreation()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            // Act
            var product = new ProductDto(
                productId,
              "Laptop",
                     "High-performance laptop",
           1299.99m,
                  10,
           categoryId,
                  "Electronics"
                  );

            // Assert
            product.Id.Should().Be(productId);
            product.Name.Should().Be("Laptop");
            product.Price.Should().Be(1299.99m);
        }
    }

    #endregion

    #region Value Equality Tests

    public class ValueEqualityTests
    {
        [Fact]
        public void ProductDto_WithSameValues_AreEqual()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            // Act
            var product1 = new ProductDto(productId, "Laptop", "Description", 999m, 10, categoryId, "Electronics");
            var product2 = new ProductDto(productId, "Laptop", "Description", 999m, 10, categoryId, "Electronics");

            // Assert
            product1.Should().Be(product2);
            product1.GetHashCode().Should().Be(product2.GetHashCode());
        }

        [Fact]
        public void ProductDto_WithDifferentIds_AreNotEqual()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var product1 = new ProductDto(Guid.NewGuid(), "Laptop", "Desc", 999m, 10, categoryId, "Electronics");
            var product2 = new ProductDto(Guid.NewGuid(), "Laptop", "Desc", 999m, 10, categoryId, "Electronics");

            // Assert
            product1.Should().NotBe(product2);
        }

        [Fact]
        public void ProductDto_WithDifferentPrices_AreNotEqual()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            // Act
            var product1 = new ProductDto(productId, "Laptop", "Desc", 999m, 10, categoryId, "Electronics");
            var product2 = new ProductDto(productId, "Laptop", "Desc", 1299m, 10, categoryId, "Electronics");

            // Assert
            product1.Should().NotBe(product2);
        }
    }

    #endregion

    #region With Expression Tests

    public class WithExpressionTests
    {
        [Fact]
        public void ProductDto_WithExpression_UpdatesPrice()
        {
            // Arrange
            var original = new ProductDto(
            Guid.NewGuid(),
                     "Laptop",
      "Description",
            999m,
       10,
                     Guid.NewGuid(),
     "Electronics"
           );

            // Act
            var modified = original with { Price = 1299m };

            // Assert
            modified.Price.Should().Be(1299m);
            modified.Name.Should().Be(original.Name);
            original.Price.Should().Be(999m); // Original unchanged
        }

        [Fact]
        public void ProductDto_WithExpression_UpdatesStock()
        {
            // Arrange
            var original = new ProductDto(
     Guid.NewGuid(),
        "Laptop",
    "Description",
                  999m,
      10,
          Guid.NewGuid(),
       "Electronics"
    );

            // Act
            var modified = original with { Stock = 5 };

            // Assert
            modified.Stock.Should().Be(5);
            original.Stock.Should().Be(10);
        }

        [Fact]
        public void ProductDto_WithExpression_UpdatesMultipleProperties()
        {
            // Arrange
            var original = new ProductDto(
             Guid.NewGuid(),
                       "Laptop",
             "Old description",
            999m,
                       10,
                Guid.NewGuid(),
                 "Electronics"
           );

            // Act
            var modified = original with
            {
                Description = "New description",
                Price = 1199m,
                Stock = 15
            };

            // Assert
            modified.Description.Should().Be("New description");
            modified.Price.Should().Be(1199m);
            modified.Stock.Should().Be(15);
            original.Description.Should().Be("Old description");
        }
    }

    #endregion

    #region Edge Cases & Boundary Tests

    public class EdgeCasesAndBoundaryTests
    {
        [Fact]
        public void ProductDto_WithNullDescription_IsValid()
        {
            // Act
            var product = new ProductDto(
          Guid.NewGuid(),
  "Product",
      null,
    99m,
     10,
        Guid.NewGuid(),
                "Category"
);

            // Assert
            product.Description.Should().BeNull();
        }

        [Fact]
        public void ProductDto_WithEmptyDescription_IsValid()
        {
            // Act
            var product = new ProductDto(
               Guid.NewGuid(),
       "Product",
      "",
               99m,
      10,
         Guid.NewGuid(),
          "Category"
          );

            // Assert
            product.Description.Should().Be("");
        }

        [Fact]
        public void ProductDto_WithZeroPrice_IsValid()
        {
            // Act
            var product = new ProductDto(
                 Guid.NewGuid(),
                     "Free Product",
                "Free sample",
          0m,
                     10,
             Guid.NewGuid(),
                "Samples"
                 );

            // Assert
            product.Price.Should().Be(0m);
        }

        [Fact]
        public void ProductDto_WithZeroStock_IsValid()
        {
            // Act
            var product = new ProductDto(
        Guid.NewGuid(),
       "Out of Stock",
     "Currently unavailable",
         99m,
        0,
            Guid.NewGuid(),
         "Electronics"
 );

            // Assert
            product.Stock.Should().Be(0);
        }

        [Fact]
        public void ProductDto_WithVeryHighPrice_IsValid()
        {
            // Act
            var product = new ProductDto(
           Guid.NewGuid(),
      "Luxury Item",
           "Premium product",
         999999.99m,
    1,
                Guid.NewGuid(),
      "Luxury"
              );

            // Assert
            product.Price.Should().Be(999999.99m);
        }

        [Fact]
        public void ProductDto_WithVeryLongName_IsValid()
        {
            // Arrange
            var longName = new string('A', 500);

            // Act
            var product = new ProductDto(
               Guid.NewGuid(),
                  longName,
               "Description",
               99m,
            10,
             Guid.NewGuid(),
                   "Category"
                   );

            // Assert
            product.Name.Should().Be(longName);
            product.Name.Should().HaveLength(500);
        }

        [Fact]
        public void ProductDto_WithSpecialCharacters_IsValid()
        {
            // Act
            var product = new ProductDto(
            Guid.NewGuid(),
       "Product with 'quotes' & <symbols> ?? ??",
                "Description with ñ, é, ü",
   99m,
      10,
         Guid.NewGuid(),
       "Category"
       );

            // Assert
            product.Name.Should().Contain("??");
            product.Description.Should().Contain("ñ");
        }

        [Fact]
        public void ProductDto_WithGuidEmpty_IsValid()
        {
            // Act
            var product = new ProductDto(
      Guid.Empty,
       "Product",
      "Description",
99m,
                10,
            Guid.Empty,
  "Category"
            );

            // Assert
            product.Id.Should().Be(Guid.Empty);
            product.CategoryId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void ProductDto_WithDecimalPrecision_IsValid()
        {
            // Act
            var product = new ProductDto(
          Guid.NewGuid(),
                "Product",
         "Description",
      19.99m,
   10,
        Guid.NewGuid(),
            "Category"
            );

            // Assert
            product.Price.Should().Be(19.99m);
        }

        [Fact]
        public void ProductDto_WithLargeStock_IsValid()
        {
            // Act
            var product = new ProductDto(
       Guid.NewGuid(),
           "Bulk Item",
               "Large inventory",
      1m,
        100000,
        Guid.NewGuid(),
   "Wholesale"
            );

            // Assert
            product.Stock.Should().Be(100000);
        }
    }

    #endregion

    #region Deconstruction Tests

    public class DeconstructionTests
    {
        [Fact]
        public void ProductDto_CanBeDeconstructed()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var product = new ProductDto(
              productId,
             "Laptop",
                   "Description",
                 1299m,
            10,
                        categoryId,
                 "Electronics"
                    );

            // Act
            var (id, name, description, price, stock, catId, catName) = product;

            // Assert
            id.Should().Be(productId);
            name.Should().Be("Laptop");
            description.Should().Be("Description");
            price.Should().Be(1299m);
            stock.Should().Be(10);
            catId.Should().Be(categoryId);
            catName.Should().Be("Electronics");
        }
    }

    #endregion

    #region ToString Tests

    public class ToStringTests
    {
        [Fact]
        public void ProductDto_ToString_ContainsKeyInformation()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new ProductDto(
               productId,
              "Laptop",
            "Description",
                1299m,
                  10,
              Guid.NewGuid(),
                      "Electronics"
                );

            // Act
            var toString = product.ToString();

            // Assert
            toString.Should().Contain(productId.ToString());
            toString.Should().Contain("Laptop");
            toString.Should().Contain("1299");
        }
    }

    #endregion

    #region Comparison Tests

    public class ComparisonTests
    {
        [Fact]
        public void ProductDto_SameReference_AreEqual()
        {
            // Arrange
            var product = new ProductDto(
              Guid.NewGuid(),
             "Laptop",
        "Description",
            1299m,
                        10,
                  Guid.NewGuid(),
                  "Electronics"
                );

            // Act & Assert
            product.Should().Be(product);
            product.Equals(product).Should().BeTrue();
        }

        [Fact]
        public void ProductDto_NullComparison_ReturnsFalse()
        {
            // Arrange
            var product = new ProductDto(
     Guid.NewGuid(),
        "Laptop",
    "Description",
                1299m,
       10,
       Guid.NewGuid(),
      "Electronics"
            );

            // Act & Assert
            product.Equals(null).Should().BeFalse();
        }
    }

    #endregion
}
