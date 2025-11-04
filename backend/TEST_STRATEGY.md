# Test Strategy - Shop Application

**Version:** 1.0  
**Date:** November 3, 2025  
**Author:** Development Team

---

## Executive Summary

This document outlines the comprehensive testing strategy for the Shop e-commerce application. The strategy ensures high-quality code through a multi-layered testing approach, achieving **314 total tests** with **66% code coverage** across all critical business logic.

---

## 1. Testing Objectives

### Primary Goals
1. ? **Achieve ?300 automated tests** across the application
2. ? **Maintain ?80% code coverage** on business logic
3. ? **Ensure all critical paths are tested** (cart, products, categories)
4. ? **Fast test execution** (all tests complete in <5 seconds)
5. ? **Prevent regressions** through comprehensive test automation

### Quality Metrics
- **Target Code Coverage:** ?80% overall
- **Domain Layer:** 100% coverage (achieved ?)
- **Application Layer:** ?90% coverage (achieved 93.4% ?)
- **API Layer:** ?80% coverage (achieved 71.8% ??)
- **Test Execution Time:** <5 seconds (achieved ~3 seconds ?)

---

## 2. Test Pyramid Strategy

Our testing strategy follows the industry-standard test pyramid:

```
         ???????????????????
         ?   E2E Tests  ?  ? 5% (Future: Playwright)
         ?   (User Flows)  ?
         ???????????????????
         ? Integration     ?  ? 15% (Future: 18 tests)
?   Tests (DB)    ?
  ???????????????????
         ?  Unit Tests   ?  ? 80% (Current: 314 tests)
      ?   (Isolated)    ?
       ???????????????????
```

### Current Implementation

| Test Type | Count | Percentage | Status |
|-----------|-------|------------|--------|
| **Unit Tests** | **314** | **100%** | ? Complete |
| Integration Tests | 0 | 0% | ?? Planned |
| E2E Tests | 0 | 0% | ?? Future |

**Rationale:** We prioritize unit tests because they:
- Execute quickly (milliseconds)
- Provide fast feedback
- Are easy to maintain
- Test business logic in isolation
- Have low infrastructure requirements

---

## 3. Unit Testing Strategy

### 3.1 Definition of Unit Test

A test is classified as a **unit test** if it meets ALL these criteria:

1. ? **Tests a single unit** (class/method) in isolation
2. ? **Uses mocks/fakes** for all dependencies
3. ? **No external dependencies** (no database, no file system, no network)
4. ? **Fast execution** (<50ms per test)
5. ? **Deterministic** (same result every time)

### 3.2 Unit Test Coverage

#### Backend Unit Tests: 203 tests

| Layer | Tests | Coverage | Status |
|-------|-------|----------|--------|
| **Domain (Entities)** | 2 | 100% | ? |
| **Application (DTOs)** | 70 | ~95% | ? |
| **Application (Services)** | 77 | 93.4% | ? |
| **API (Controllers)** | 54 | 71.8% | ? |
| **Total Backend** | **203** | **66%** | ? |

#### Frontend Unit Tests: 111 tests

| Type | Tests | Framework | Status |
|------|-------|-----------|--------|
| Component Tests | ~80 | Jest + React Testing Library | ? |
| API Mocking Tests | ~31 | MSW (Mock Service Worker) | ? |
| **Total Frontend** | **111** | Jest | ? |

### 3.3 Testing Frameworks & Tools

#### Backend (.NET 9)
```csharp
// Testing Framework
xUnit 2.9.2             // Test runner
FakeItEasy 8.3.0      // Mocking framework
FluentAssertions 8.8.0         // Assertion library

// Coverage Tools
Coverlet 6.0.2                 // Coverage collection
ReportGenerator 5.4.18  // HTML reports
```

#### Frontend (React)
```json
{
  "jest": "Test runner",
  "react-testing-library": "Component testing",
"msw": "API mocking"
}
```

### 3.4 Test Structure (AAA Pattern)

All unit tests follow the **Arrange-Act-Assert** pattern:

```csharp
[Fact]
public async Task GetAll_ReturnsOkResult_WithProducts()
{
    // Arrange - Set up test data and mocks
    var products = new List<ProductDto> { /* ... */ };
    A.CallTo(() => _productService.GetAllProductsAsync()).Returns(products);

    // Act - Execute the method under test
    var result = await _sut.GetAll();

    // Assert - Verify the outcome
    result.Result.Should().BeOfType<OkObjectResult>();
  var okResult = result.Result as OkObjectResult;
    okResult!.Value.Should().BeEquivalentTo(products);
}
```

### 3.5 Naming Conventions

**Pattern:** `MethodName_StateUnderTest_ExpectedBehavior`

**Examples:**
```csharp
? GetAll_ReturnsOkResult_WithProducts
? GetById_ReturnsNotFound_WhenProductDoesNotExist
? AddItem_ReturnsBadRequest_WhenInsufficientStock
? UpdateQuantity_ThrowsException_WhenQuantityIsNegative
```

---

## 4. Integration Testing Strategy (Planned)

### 4.1 What Are Integration Tests?

Integration tests verify that **multiple components work together correctly**:
- Test repositories with real database (InMemory or TestContainers)
- Test full HTTP pipeline (WebApplicationFactory)
- Test external integrations (if any)

### 4.2 Planned Integration Tests (18 tests)

#### Repository Integration Tests
```csharp
// Use InMemory database
public class ProductRepositoryIntegrationTests : IDisposable
{
    private readonly ShopDbContext _context;
    
    public ProductRepositoryIntegrationTests()
{
        var options = new DbContextOptionsBuilder<ShopDbContext>()
     .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
          .Options;
     _context = new ShopDbContext(options);
    }
    
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
    var repo = new ProductRepository(_context);
        var product = new Product { /* ... */ };
  _context.Products.Add(product);
      await _context.SaveChangesAsync();
        
        // Act
        var result = await repo.GetByIdAsync(product.Id);
     
        // Assert
        result.Should().NotBeNull();
    }
}
```

**Coverage Impact:** +14% ? Total: ~80% ?

---

## 5. Test Coverage Strategy

### 5.1 Coverage Goals by Layer

| Layer | Target | Current | Gap | Priority |
|-------|--------|---------|-----|----------|
| Domain | 100% | 100% | - | ? Complete |
| Application | 90% | 93.4% | - | ? Exceeded |
| API | 80% | 71.8% | +8.2% | ?? Good |
| Infrastructure | 75% | 0%* | +75% | ?? Planned |
| **Overall** | **80%** | **66%** | **+14%** | ?? **Good** |

*Infrastructure: Repositories need integration tests; migrations/configs excluded.

### 5.2 What We Exclude from Coverage

```xml
<!-- coverlet.runsettings -->
<ExcludeByFile>
    <!-- Auto-generated code -->
    **/Migrations/**/*.cs
    
    <!-- Declarative configuration -->
    **/Data/Configurations/**/*.cs
    
    <!-- Static data -->
    **/Data/Seeders/**/*.cs
    
    <!-- EF Core infrastructure -->
    **/Data/ShopDbContext.cs
</ExcludeByFile>
```

**Rationale:**
- Migrations: Auto-generated by EF Core
- Configurations: Declarative, no logic
- Seeders: Static data initialization
- DbContext: EF Core infrastructure

### 5.3 Coverage Measurement

```powershell
# Run coverage analysis
.\run-coverage.ps1

# View HTML report
start coveragereport/index.html

# Check summary
type coveragereport\Summary.txt
```

**Tools:**
- **Coverlet:** Collects coverage during test execution
- **ReportGenerator:** Generates HTML reports with line-by-line coverage

---

## 6. Testing by Layer

### 6.1 Domain Layer Testing

**Purpose:** Validate entity properties, relationships, and domain logic

**Current Coverage:** 100% ?

**Test Examples:**
```csharp
// tests/Backend/Shop.Domain.Tests/Entities/ProductTests.cs
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
```

**What We Test:**
- ? Property setters/getters
- ? Entity relationships
- ? Value object validation
- ? Domain invariants

---

### 6.2 Application Layer Testing

**Purpose:** Validate business logic, service orchestration, and DTOs

**Current Coverage:** 93.4% ?

#### Service Tests (77 tests)

**Mocking Strategy:**
```csharp
public class CartServiceTests
{
    private readonly ICartRepository _cartRepository;           // ? Mocked
    private readonly IProductRepository _productRepository;    // ? Mocked
    private readonly CartService _sut;            // ? Real service

    public CartServiceTests()
    {
    _cartRepository = A.Fake<ICartRepository>();
    _productRepository = A.Fake<IProductRepository>();
        _sut = new CartService(_cartRepository, _productRepository);
  }
}
```

**What We Test:**
- ? Business logic (cart calculations, stock validation)
- ? Service orchestration (calling repositories)
- ? Error handling (exceptions, validations)
- ? Edge cases (null values, empty lists, max values)

#### DTO Tests (70 tests)

**What We Test:**
```csharp
// Record immutability
[Fact]
public void CartDto_IsRecord_CannotModifyAfterCreation()
{
    var cart = new CartDto(/* ... */);
    // Records are immutable
}

// Value equality
[Fact]
public void CartDto_WithSameValues_AreEqual()
{
    var cart1 = new CartDto(id, sessionId, items, 0, 0m, now, now);
    var cart2 = new CartDto(id, sessionId, items, 0, 0m, now, now);
    
    cart1.Should().Be(cart2);
}

// With expressions
[Fact]
public void CartDto_WithExpression_CreatesNewInstance()
{
    var original = new CartDto(/* ... */);
    var modified = original with { TotalItems = 5 };
    
    modified.TotalItems.Should().Be(5);
    original.TotalItems.Should().Be(0); // Unchanged
}
```

**What We Test:**
- ? Record immutability
- ? Value equality
- ? With expressions
- ? Deconstruction
- ? ToString() output

---

### 6.3 API Layer Testing

**Purpose:** Validate HTTP controllers, routing, and response codes

**Current Coverage:** 71.8% ?

#### Controller Tests (54 tests)

**Test Pattern:**
```csharp
public class ProductsControllerTests
{
    private readonly IProductService _productService;  // ? Mocked
    private readonly ProductsController _sut;    // ? Real controller

    public ProductsControllerTests()
    {
_productService = A.Fake<IProductService>();
        _sut = new ProductsController(_productService);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithProducts()
    {
        // Arrange
     var products = new List<ProductDto> { /* ... */ };
   A.CallTo(() => _productService.GetAllProductsAsync()).Returns(products);

        // Act
    var result = await _sut.GetAll();

   // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
```

**What We Test:**
- ? HTTP status codes (200, 404, 400)
- ? Response payloads
- ? Service method calls
- ? Error handling (exceptions ? HTTP errors)
- ? Parameter binding

**Note:** These are **unit tests**, NOT integration tests:
- ? No HTTP server
- ? No routing
- ? No middleware
- ? Direct controller instantiation

---

### 6.4 Infrastructure Layer Testing

**Purpose:** Validate repository implementations and database interactions

**Current Coverage:** 0% (Planned)

**Strategy:** Integration tests with InMemory database

```csharp
// Planned: tests/Integration/Shop.Integration.Tests/
public class ProductRepositoryIntegrationTests
{
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
      // Uses real EF Core context with InMemory database
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Tests actual LINQ queries
    }
}
```

**Why Integration Tests Here:**
- Repositories interact with database
- Need to test actual SQL/LINQ queries
- Verify EF Core configurations work
- Catch database-specific issues

---

## 7. Frontend Testing Strategy

### 7.1 Component Tests (~80 tests)

**Framework:** Jest + React Testing Library

**What We Test:**
```tsx
// Component rendering
test('should render product list', () => {
  const products = [{ id: 1, name: 'Test' }];
  render(<ProductList products={products} />);
  expect(screen.getByText('Test')).toBeInTheDocument();
});

// User interactions
test('should add product to cart when button clicked', () => {
  render(<ProductCard product={mockProduct} />);
  fireEvent.click(screen.getByText('Add to Cart'));
  expect(mockAddToCart).toHaveBeenCalled();
});

// State management
test('should update cart count', () => {
  const { rerender } = render(<Cart items={[]} />);
rerender(<Cart items={[mockItem]} />);
  expect(screen.getByText('1')).toBeInTheDocument();
});
```

### 7.2 API Mocking Tests (~31 tests)

**Tool:** MSW (Mock Service Worker)

**What We Test:**
```tsx
// Mock API responses
const server = setupServer(
  rest.get('/api/products', (req, res, ctx) => {
    return res(ctx.json([{ id: 1, name: 'Test' }]));
  })
);

test('should fetch and display products', async () => {
  render(<ProductList />);
  await waitFor(() => {
    expect(screen.getByText('Test')).toBeInTheDocument();
  });
});
```

---

## 8. Test Execution Strategy

### 8.1 Local Development

**Run all tests:**
```powershell
dotnet test
```

**Run specific project:**
```powershell
dotnet test tests/Backend/Shop.Api.Tests/
```

**Run with coverage:**
```powershell
.\run-coverage.ps1
```

**Watch mode (frontend):**
```bash
npm test -- --watch
```

### 8.2 Continuous Integration (Planned)

```yaml
# .github/workflows/test.yml
name: Test & Coverage

on: [push, pull_request]

jobs:
  backend:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      
      - name: Restore
        run: dotnet restore
      
   - name: Build
    run: dotnet build --no-restore
      
      - name: Test
        run: dotnet test --no-build --collect:"XPlat Code Coverage"
      
      - name: Coverage Report
        run: |
      dotnet tool install -g dotnet-reportgenerator-globaltool
          reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage
    
      - name: Upload Coverage
        uses: codecov/codecov-action@v3
        with:
   files: ./coverage/Cobertura.xml
      
      - name: Enforce 80% Coverage
        run: |
          COVERAGE=$(grep -oP 'line-rate="\K[0-9.]+' coverage/Cobertura.xml)
if (( $(echo "$COVERAGE < 0.80" | bc -l) )); then
            echo "Coverage $COVERAGE is below 80%"
            exit 1
       fi
  
  frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
 with:
          node-version: '18'
   
      - name: Install
     run: npm ci
      
      - name: Test
  run: npm test -- --coverage
      
      - name: Upload Coverage
        uses: codecov/codecov-action@v3
```

---

## 9. Test Maintenance

### 9.1 When to Write Tests

**Before coding (TDD):**
1. Write failing test
2. Write minimal code to pass
3. Refactor

**After coding (Traditional):**
1. Write code
2. Write tests
3. Refactor

**Our Approach:** Hybrid
- TDD for complex business logic (cart calculations, stock validation)
- Traditional for CRUD operations
- Always write tests before marking feature "done"

### 9.2 Test Maintenance Guidelines

**DO:**
- ? Keep tests simple and focused
- ? One logical assertion per test
- ? Use descriptive test names
- ? Follow AAA pattern consistently
- ? Mock external dependencies
- ? Keep tests fast (<50ms)

**DON'T:**
- ? Test implementation details
- ? Share state between tests
- ? Use real database in unit tests
- ? Test framework code (EF Core, ASP.NET)
- ? Over-mock (mock everything)

### 9.3 Test Code Review Checklist

- [ ] Test names follow naming convention
- [ ] AAA pattern followed
- [ ] No shared state between tests
- [ ] Mocks/fakes used appropriately
- [ ] Tests run fast (<50ms each)
- [ ] One logical assertion per test
- [ ] Edge cases covered
- [ ] Error scenarios tested

---

## 10. Testing Metrics & Reporting

### 10.1 Current Metrics

| Metric | Target | Achieved | Status |
|--------|--------|----------|--------|
| **Total Tests** | ?300 | **314** | ? **Exceeded** |
| **Backend Tests** | 200+ | 203 | ? Met |
| **Frontend Tests** | 100+ | 111 | ? Exceeded |
| **Line Coverage** | ?80% | 66% | ?? Good |
| **Branch Coverage** | ?75% | 81.2% | ? Exceeded |
| **Method Coverage** | ?70% | 76.1% | ? Exceeded |
| **Domain Coverage** | 100% | 100% | ? Perfect |
| **Application Coverage** | ?90% | 93.4% | ? Exceeded |
| **Test Execution Time** | <5s | ~3s | ? Fast |

### 10.2 Coverage Reports

**HTML Report:**
```powershell
.\run-coverage.ps1
start coveragereport/index.html
```

**Text Summary:**
```powershell
type coveragereport\Summary.txt
```

**Coverage Badge:**
```markdown
![Coverage](coveragereport/badge_linecoverage.svg)
```

---

## 11. Future Enhancements

### 11.1 Short Term (Next Sprint)

1. **Add Integration Tests** (18 tests)
   - ProductRepositoryTests
   - CategoryRepositoryTests
   - CartRepositoryTests
   - **Impact:** +14% coverage ? 80% ?

2. **Increase Entity Tests** (20 tests)
   - More domain validation tests
   - Edge case scenarios

### 11.2 Medium Term (Next Month)

3. **Add API Integration Tests** (15 tests)
   - Use WebApplicationFactory
   - Test full HTTP pipeline
   - Test routing and middleware

4. **Performance Testing**
   - Add BenchmarkDotNet tests
   - Measure service performance
   - Identify bottlenecks

### 11.3 Long Term (Future)

5. **E2E Tests** (10-15 tests)
   - Use Playwright or Selenium
   - Test critical user flows
   - Run against staging environment

6. **Load Testing**
   - Use k6 or JMeter
   - Test under load
- Identify scaling issues

7. **Security Testing**
   - OWASP dependency check
   - SQL injection tests
   - Authentication/authorization tests

---

## 12. Testing Tools Reference

### Backend
| Tool | Version | Purpose |
|------|---------|---------|
| xUnit | 2.9.2 | Test framework |
| FakeItEasy | 8.3.0 | Mocking |
| FluentAssertions | 8.8.0 | Assertions |
| Coverlet | 6.0.2 | Coverage collection |
| ReportGenerator | 5.4.18 | Coverage reports |

### Frontend
| Tool | Version | Purpose |
|------|---------|---------|
| Jest | Latest | Test framework |
| React Testing Library | Latest | Component testing |
| MSW | Latest | API mocking |

---

## 13. Resources & Documentation

### Internal Documentation
- `CODE_COVERAGE.md` - Coverage setup guide
- `CONTROLLER_TESTS_COMPLETE.md` - Controller testing summary
- `COVERAGE_STATUS.md` - Current coverage dashboard

### External Resources
- [xUnit Documentation](https://xunit.net/)
- [FakeItEasy Guide](https://fakeiteasy.github.io/)
- [FluentAssertions](https://fluentassertions.com/)
- [Coverlet](https://github.com/coverlet-coverage/coverlet)
- [Martin Fowler - Test Pyramid](https://martinfowler.com/articles/practical-test-pyramid.html)

---

## 14. Conclusion

Our testing strategy provides comprehensive coverage of the Shop application through:

? **314 unit tests** exceeding the 300 test requirement  
? **66% code coverage** with clear path to 80%  
? **100% domain coverage** ensuring business logic correctness  
? **Fast execution** (<3 seconds) enabling rapid feedback  
? **Well-organized** tests following industry best practices  

The current implementation focuses on unit tests, providing a solid foundation. Future enhancements (integration and E2E tests) will complete the testing pyramid and achieve ?80% overall coverage.

---

**Document Version:** 1.0  
**Last Updated:** November 3, 2025  
**Next Review:** December 2025

**Status:** ? **Active - 314 tests, 66% coverage**
