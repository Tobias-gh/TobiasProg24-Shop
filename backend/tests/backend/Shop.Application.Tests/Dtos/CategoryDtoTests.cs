using FluentAssertions;
using Shop.Application.Dtos;

namespace Shop.Application.Tests.Dtos;

public class CategoryDtoTests
{
    #region Record Immutability Tests

    public class RecordImmutabilityTests
    {
        [Fact]
        public void CategoryDto_IsRecord_CannotModifyAfterCreation()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var category = new CategoryDto(
          categoryId,
     "Electronics",
       "Electronic devices and gadgets",
        25
        );

            // Assert
            category.Id.Should().Be(categoryId);
            category.Name.Should().Be("Electronics");
            category.ProductCount.Should().Be(25);
        }
    }

    #endregion

    #region Value Equality Tests

    public class ValueEqualityTests
    {
        [Fact]
        public void CategoryDto_WithSameValues_AreEqual()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var category1 = new CategoryDto(categoryId, "Electronics", "Description", 10);
            var category2 = new CategoryDto(categoryId, "Electronics", "Description", 10);

            // Assert
            category1.Should().Be(category2);
            category1.GetHashCode().Should().Be(category2.GetHashCode());
        }

        [Fact]
        public void CategoryDto_WithDifferentIds_AreNotEqual()
        {
            // Act
            var category1 = new CategoryDto(Guid.NewGuid(), "Electronics", "Desc", 10);
            var category2 = new CategoryDto(Guid.NewGuid(), "Electronics", "Desc", 10);

            // Assert
            category1.Should().NotBe(category2);
        }

        [Fact]
        public void CategoryDto_WithDifferentNames_AreNotEqual()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var category1 = new CategoryDto(categoryId, "Electronics", "Desc", 10);
            var category2 = new CategoryDto(categoryId, "Books", "Desc", 10);

            // Assert
            category1.Should().NotBe(category2);
        }

        [Fact]
        public void CategoryDto_WithDifferentProductCounts_AreNotEqual()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var category1 = new CategoryDto(categoryId, "Electronics", "Desc", 10);
            var category2 = new CategoryDto(categoryId, "Electronics", "Desc", 20);

            // Assert
            category1.Should().NotBe(category2);
        }
    }

    #endregion

    #region With Expression Tests

    public class WithExpressionTests
    {
        [Fact]
        public void CategoryDto_WithExpression_UpdatesName()
        {
            // Arrange
            var original = new CategoryDto(
            Guid.NewGuid(),
          "Electronics",
              "Electronic devices",
              10
         );

            // Act
            var modified = original with { Name = "Updated Electronics" };

            // Assert
            modified.Name.Should().Be("Updated Electronics");
            original.Name.Should().Be("Electronics");
        }

        [Fact]
        public void CategoryDto_WithExpression_UpdatesProductCount()
        {
            // Arrange
            var original = new CategoryDto(
                 Guid.NewGuid(),
                  "Electronics",
                  "Description",
                10
                       );

            // Act
            var modified = original with { ProductCount = 25 };

            // Assert
            modified.ProductCount.Should().Be(25);
            original.ProductCount.Should().Be(10);
        }

        [Fact]
        public void CategoryDto_WithExpression_UpdatesDescription()
        {
            // Arrange
            var original = new CategoryDto(
               Guid.NewGuid(),
               "Electronics",
               "Old description",
            10
               );

            // Act
            var modified = original with { Description = "New description" };

            // Assert
            modified.Description.Should().Be("New description");
            original.Description.Should().Be("Old description");
        }

        [Fact]
        public void CategoryDto_WithExpression_UpdatesMultipleProperties()
        {
            // Arrange
            var original = new CategoryDto(
        Guid.NewGuid(),
          "Electronics",
          "Description",
          10
       );

            // Act
            var modified = original with
            {
                Name = "Updated Name",
                Description = "Updated Description",
                ProductCount = 50
            };

            // Assert
            modified.Name.Should().Be("Updated Name");
            modified.Description.Should().Be("Updated Description");
            modified.ProductCount.Should().Be(50);
            original.Name.Should().Be("Electronics");
        }
    }

    #endregion

    #region Edge Cases & Boundary Tests

    public class EdgeCasesAndBoundaryTests
    {
        [Fact]
        public void CategoryDto_WithNullDescription_IsValid()
        {
            // Act
            var category = new CategoryDto(
        Guid.NewGuid(),
         "Category",
            null,
                10
            );

            // Assert
            category.Description.Should().BeNull();
        }

        [Fact]
        public void CategoryDto_WithEmptyDescription_IsValid()
        {
            // Act
            var category = new CategoryDto(
         Guid.NewGuid(),
        "Category",
        "",
        10
    );

            // Assert
            category.Description.Should().Be("");
        }

        [Fact]
        public void CategoryDto_WithZeroProductCount_IsValid()
        {
            // Act
            var category = new CategoryDto(
        Guid.NewGuid(),
                 "Empty Category",
          "No products yet",
       0
        );

            // Assert
            category.ProductCount.Should().Be(0);
        }

        [Fact]
        public void CategoryDto_WithLargeProductCount_IsValid()
        {
            // Act
            var category = new CategoryDto(
 Guid.NewGuid(),
          "Popular Category",
          "Many products",
   10000
     );

            // Assert
            category.ProductCount.Should().Be(10000);
        }

        [Fact]
        public void CategoryDto_WithVeryLongName_IsValid()
        {
            // Arrange
            var longName = new string('A', 500);

            // Act
            var category = new CategoryDto(
     Guid.NewGuid(),
             longName,
      "Description",
         10
          );

            // Assert
            category.Name.Should().Be(longName);
            category.Name.Should().HaveLength(500);
        }

        [Fact]
        public void CategoryDto_WithSpecialCharactersInName_IsValid()
        {
            // Act
            var category = new CategoryDto(
     Guid.NewGuid(),
              "Category with 'quotes' & <symbols> ?? ??",
        "Description with ñ, é, ü",
               10
            );

            // Assert
            category.Name.Should().Contain("??");
            category.Description.Should().Contain("ñ");
        }

        [Fact]
        public void CategoryDto_WithVeryLongDescription_IsValid()
        {
            // Arrange
            var longDescription = new string('B', 5000);

            // Act
            var category = new CategoryDto(
      Guid.NewGuid(),
        "Category",
            longDescription,
               10
            );

            // Assert
            category.Description.Should().HaveLength(5000);
        }

        [Fact]
        public void CategoryDto_WithGuidEmpty_IsValid()
        {
            // Act
            var category = new CategoryDto(
        Guid.Empty,
     "Category",
       "Description",
         10
       );

            // Assert
            category.Id.Should().Be(Guid.Empty);
        }

        [Fact]
        public void CategoryDto_WithNegativeProductCount_IsValid()
        {
            // Note: While technically possible with records, business logic should prevent this
            // Act
            var category = new CategoryDto(
                 Guid.NewGuid(),
            "Category",
                "Description",
              -1
              );

            // Assert
            category.ProductCount.Should().Be(-1);
        }
    }

    #endregion

    #region Deconstruction Tests

    public class DeconstructionTests
    {
        [Fact]
        public void CategoryDto_CanBeDeconstructed()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new CategoryDto(
   categoryId,
      "Electronics",
    "Electronic devices",
      25
      );

            // Act
            var (id, name, description, productCount) = category;

            // Assert
            id.Should().Be(categoryId);
            name.Should().Be("Electronics");
            description.Should().Be("Electronic devices");
            productCount.Should().Be(25);
        }
    }

    #endregion

    #region ToString Tests

    public class ToStringTests
    {
        [Fact]
        public void CategoryDto_ToString_ContainsKeyInformation()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new CategoryDto(
            categoryId,
                    "Electronics",
               "Electronic devices",
                25
                      );

            // Act
            var toString = category.ToString();

            // Assert
            toString.Should().Contain(categoryId.ToString());
            toString.Should().Contain("Electronics");
            toString.Should().Contain("25");
        }
    }

    #endregion

    #region Comparison Tests

    public class ComparisonTests
    {
        [Fact]
        public void CategoryDto_SameReference_AreEqual()
        {
            // Arrange
            var category = new CategoryDto(
                Guid.NewGuid(),
              "Electronics",
           "Description",
                     10
          );

            // Act & Assert
            category.Should().Be(category);
            category.Equals(category).Should().BeTrue();
        }

        [Fact]
        public void CategoryDto_NullComparison_ReturnsFalse()
        {
            // Arrange
            var category = new CategoryDto(
             Guid.NewGuid(),
         "Electronics",
            "Description",
            10
             );

            // Act & Assert
            category.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void CategoryDto_DifferentType_ReturnsFalse()
        {
            // Arrange
            var category = new CategoryDto(
           Guid.NewGuid(),
            "Electronics",
        "Description",
           10
             );
            var otherObject = "Not a CategoryDto";

            // Act & Assert
            category.Equals(otherObject).Should().BeFalse();
        }
    }

    #endregion

    #region Business Logic Tests

    public class BusinessLogicTests
    {
        [Fact]
        public void CategoryDto_WithNoProducts_HasZeroCount()
        {
            // Act
            var category = new CategoryDto(
                      Guid.NewGuid(),
                  "New Category",
              "Just created",
               0
                );

            // Assert
            category.ProductCount.Should().Be(0);
        }

        [Fact]
        public void CategoryDto_ProductCountReflectsInventory()
        {
            // This test demonstrates that ProductCount is a snapshot
            // and doesn't dynamically update

            // Arrange
            var category = new CategoryDto(Guid.NewGuid(), "Electronics", "Desc", 10);

            // Act
            var updatedCategory = category with { ProductCount = 15 };

            // Assert
            category.ProductCount.Should().Be(10);
            updatedCategory.ProductCount.Should().Be(15);
        }
    }

    #endregion
}
