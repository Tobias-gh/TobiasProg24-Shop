# Shop Frontend - Testing Summary

## Overview

The shop-frontend project implements a comprehensive testing strategy covering unit tests, integration tests, and end-to-end (E2E) tests. This document provides a complete overview of the testing infrastructure, coverage, and best practices.

---

## Test Infrastructure

### Testing Frameworks

#### 1. **Vitest** (Unit & Integration Tests)
- **Version**: ^4.0.6
- **Purpose**: Unit and integration testing for React components, utilities, and API clients
- **Configuration**: `config/vitest.config.ts`
- **Environment**: jsdom (simulates browser environment)
- **Coverage Provider**: v8

#### 2. **Playwright** (E2E Tests)
- **Version**: ^1.56.1
- **Purpose**: End-to-end browser testing
- **Configuration**: `config/playwright.config.ts`
- **Browser**: Chromium (Desktop Chrome)
- **Base URL**: http://localhost:5173

#### 3. **Testing Library**
- **@testing-library/react**: ^16.3.0
- **@testing-library/user-event**: ^14.6.1
- **@testing-library/jest-dom**: ^6.9.1
- **Purpose**: React component testing with user-centric queries

---

## Test Scripts

```json
{
  "test": "vitest --config config/vitest.config.ts",
  "test:ui": "vitest --ui --config config/vitest.config.ts",
  "test:coverage": "vitest --coverage --config config/vitest.config.ts",
  "test:run": "vitest run --config config/vitest.config.ts",
  "test:e2e": "playwright test --config config/playwright.config.ts",
  "test:e2e:ui": "playwright test --ui --config config/playwright.config.ts",
  "test:e2e:headed": "playwright test --headed --config config/playwright.config.ts",
  "test:e2e:debug": "playwright test --debug --config config/playwright.config.ts"
}
```

---

## Test Structure

### Directory Organization

```
shop-frontend/
├── tests/                          # Unit & Integration Tests
│   ├── setup.ts                   # Global test setup
│   ├── api/                       # API client tests
│   │   ├── carts.api.test.ts
│   │   ├── categories.api.test.ts
│   │   └── products.api.test.ts
│   ├── components/                # Component tests
│   │   └── ui/
│   │       ├── badge.test.tsx
│   │       ├── button.test.tsx
│   │       └── card.test.tsx
│   ├── lib/                       # Utility tests
│   │   └── utils.test.ts
│   └── helpers/                   # Test utilities
│       ├── mockData.ts
│       └── renderWithProviders.tsx
└── e2e/                           # End-to-End Tests
    ├── example.spec.ts
    ├── homepage.spec.ts
    ├── product-details.spec.ts
    └── shopping-cart.spec.ts
```

---

## Test Coverage

### 1. API Client Tests (`tests/api/`)

#### **Carts API** (`carts.api.test.ts`)
- ✅ **99 test cases** covering all cart operations
- **Coverage Areas**:
  - `getCart()`: Fetch cart by session ID, handle 404, server errors
  - `addItem()`: Add items, validate quantities, stock validation, error handling
  - `updateItem()`: Update quantities, validate constraints, item not found
  - `removeItem()`: Remove items, handle last item, server errors
  - `clearCart()`: Clear all items, void return handling
  - `getSessionId()`: Generate/retrieve session ID, localStorage persistence

**Key Test Scenarios**:
- Empty cart retrieval
- Adding items with various quantities (positive, negative, zero)
- Stock validation (out of stock, exceeds available)
- Quantity updates with constraints
- Network errors (timeout, 500, 404, 400)
- Session ID generation and persistence

#### **Categories API** (`categories.api.test.ts`)
- ✅ **13 test cases**
- **Coverage Areas**:
  - `getAll()`: Fetch all categories, empty arrays, server errors
  - `getById()`: Fetch by ID, 404 handling, invalid GUID

**Key Test Scenarios**:
- Empty category list
- Product count validation
- Required fields validation (id, name, description, productCount)
- Network and server error handling

#### **Products API** (`products.api.test.ts`)
- ✅ **21 test cases**
- **Coverage Areas**:
  - `getAll()`: Fetch all products, type validation
  - `getById()`: Fetch by ID, 404 handling, empty ID
  - `getByCategory()`: Filter by category, query parameters

**Key Test Scenarios**:
- Empty product list
- Zero stock products
- Category filtering with query parameters
- Data type validation (string, number)
- Required fields validation

---

### 2. UI Component Tests (`tests/components/ui/`)

#### **Button Component** (`button.test.tsx`)
- ✅ **20+ test cases**
- **Coverage Areas**:
  - Rendering with different variants (default, destructive, outline, secondary, ghost, link)
  - Size variants (sm, lg, icon)
  - Disabled state
  - Click handlers
  - Keyboard navigation (Enter, Space)
  - Custom className
  - `asChild` prop (polymorphic behavior)

**Variants Tested**:
- `default`: Primary button style
- `destructive`: Delete/danger actions
- `outline`: Bordered button
- `secondary`: Secondary actions
- `ghost`: Minimal style
- `link`: Link-style button

**Sizes Tested**:
- `sm`: Small (h-9)
- `default`: Medium (h-10)
- `lg`: Large (h-11)
- `icon`: Icon-only (h-10, w-10)

#### **Badge Component** (`badge.test.tsx`)
- ✅ **12 test cases**
- **Coverage Areas**:
  - Variant styles (default, secondary, destructive, outline)
  - Custom className
  - Number and text content
  - Use cases (stock status, category labels, cart count)

#### **Card Component** (`card.test.tsx`)
- ✅ **13 test cases** covering all card sub-components
- **Coverage Areas**:
  - `Card`: Container with border and rounded corners
  - `CardHeader`: Header section with padding
  - `CardTitle`: H3 heading with font styles
  - `CardDescription`: Muted text description
  - `CardContent`: Main content area with padding
  - `CardFooter`: Footer with flex layout
  - Complete card structure integration

---

### 3. Utility Tests (`tests/lib/`)

#### **Class Name Utility** (`utils.test.ts`)
- ✅ **8 test cases**
- **Coverage Areas**:
  - `cn()` function: Merge class names with Tailwind conflict resolution
  - Conditional classes with objects
  - Array of classes
  - Undefined/null handling
  - Tailwind merge (e.g., `px-2` + `px-4` = `px-4`)

---

### 4. End-to-End Tests (`e2e/`)

#### **Homepage** (`homepage.spec.ts`)
- ✅ **5 test scenarios**
- **User Flows**:
  - Page load with title and subtitle
  - Header navigation visibility
  - Product grid display
  - Navigation to product details
  - Navigation to cart page

**Data Test IDs**:
- `home-page`, `page-title`, `page-subtitle`
- `header`, `logo-link`, `home-link`, `cart-link`
- `product-list`, `product-card`, `view-details-button`

#### **Shopping Cart** (`shopping-cart.spec.ts`)
- ✅ **10 comprehensive test scenarios**
- **User Flows**:
  - Empty cart display
  - Add product from homepage
  - View cart with items
  - Increase/decrease quantity
  - Remove individual items
  - Clear entire cart
  - Total price calculation
  - Continue shopping navigation

**Cart Operations Tested**:
- Add to cart (badge count updates)
- Quantity increment/decrement
- Item removal (cart becomes empty)
- Bulk clear operation
- Price calculation accuracy

#### **Product Details** (`product-details.spec.ts`)
- ✅ **4 test scenarios**
- **User Flows**:
  - Display product details page
  - Add to cart from details
  - Navigate back to homepage
  - Stock information display

---

## Test Setup & Configuration

### Global Setup (`tests/setup.ts`)

```typescript
// Features configured:
- afterEach cleanup (React Testing Library)
- window.matchMedia mock
- localStorage mock with get/set/remove/clear
- crypto.randomUUID mock for session ID generation
```

### Vitest Configuration Highlights

```typescript
{
  environment: 'jsdom',
  globals: true,
  setupFiles: ['./tests/setup.ts'],
  
  // Test file patterns
  include: ['tests/**/*.{test,spec}.{ts,tsx}'],
  exclude: ['**/node_modules/**', '**/e2e/**'],
  
  // Coverage settings
  coverage: {
    provider: 'v8',
    reporter: ['text', 'json', 'html', 'lcov'],
    thresholds: {
      lines: 80,
      functions: 80,
      branches: 80,
      statements: 80,
    },
    exclude: [
      'tests/', 'e2e/', '**/*.test.{ts,tsx}',
      'src/main.tsx', 'src/App.tsx',
      'src/components/**', 'src/pages/**'
    ],
    include: [
      'src/**/*.{ts,tsx}',
      'src/lib/**/*.{ts,tsx}',
      'src/components/ui/**/*.{ts,tsx}'
    ]
  }
}
```

### Playwright Configuration Highlights

```typescript
{
  testDir: './e2e',
  fullyParallel: true,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  
  use: {
    baseURL: 'http://localhost:5173',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure'
  },
  
  webServer: {
    command: 'npm run dev',
    url: 'http://localhost:5173',
    reuseExistingServer: !process.env.CI,
    timeout: 120000
  }
}
```

---

## Mock Data Strategy

### Mock Data Utilities (`tests/helpers/mockData.ts`)

**Provided Mocks**:
- `mockProducts`: 4 sample products (Laptop, Mouse, Coffee Mug, Notebook)
- `mockCategories`: 3 categories (Electronics, Home & Kitchen, Office Supplies)
- `mockCart`: Cart with 2 items (Laptop + Mouse)
- `mockEmptyCart`: Empty cart state

**Factory Functions**:
- `createMockProduct(overrides)`: Generate custom products
- `createMockCartItem(overrides)`: Generate custom cart items

**Usage Example**:
```typescript
import { mockProducts, createMockProduct } from '../helpers/mockData';

// Use predefined mock
const product = mockProducts[0];

// Create custom mock
const outOfStock = createMockProduct({ stock: 0, name: 'Sold Out Item' });
```

---

## Mocking Strategy

### API Mocking (Vitest Tests)

**Tool**: `axios-mock-adapter` (^2.1.0)

```typescript
import MockAdapter from 'axios-mock-adapter';
import { apiClient } from '../../src/api/apiClient';

let mock: MockAdapter;

beforeEach(() => {
  mock = new MockAdapter(apiClient);
});

// Mock successful response
mock.onGet('/api/products').reply(200, mockProducts);

// Mock error
mock.onGet('/api/products/invalid').reply(404);

// Mock network error
mock.onGet('/api/products').networkError();
```

### Component Mocking (Vitest)

```typescript
import { vi } from 'vitest';

// Mock function
const handleClick = vi.fn();

// Mock module
vi.mock('../../src/api/apiClient');
```

---

## Testing Best Practices

### 1. **Test Organization (AAA Pattern)**
```typescript
it('should add item to cart successfully', async () => {
  // Arrange
  mock.onPost(`/api/carts/${sessionId}/items`).reply(200, updatedCart);

  // Act
  const result = await cartsApi.addItem(sessionId, request);

  // Assert
  expect(result).toEqual(updatedCart);
});
```

### 2. **User-Centric Testing**
- Use `@testing-library/react` queries (getByRole, getByTestId)
- Test user interactions (click, keyboard navigation)
- Avoid implementation details

### 3. **Comprehensive Error Testing**
- Test all error states (400, 404, 500)
- Network errors (timeout, connection failure)
- Edge cases (empty strings, zero values, negative numbers)

### 4. **E2E Test Reliability**
- Use `waitForTimeout()` for async operations
- Verify element visibility before interaction
- Test complete user flows, not isolated actions

### 5. **Data-TestId Convention**
```typescript
// Component
<div data-testid="product-card">...</div>

// Test
expect(page.getByTestId('product-card')).toBeVisible();
```

---

## Coverage Thresholds

### Current Targets (80%)
- **Lines**: 80%
- **Functions**: 80%
- **Branches**: 80%
- **Statements**: 80%

### Excluded from Coverage
- Test files (`**/*.test.{ts,tsx}`, `**/*.spec.{ts,tsx}`)
- E2E tests (`e2e/`)
- Entry point (`src/main.tsx`)
- Root App component (`src/App.tsx`)
- Page components (`src/pages/**`)
- Complex UI components (`src/components/**`)
- Index files (`**/index.ts`)
- Config files (`**/*.config.{ts,js}`)

---

## CI/CD Integration

### GitHub Actions (`.github/workflows/playwright.yml`)
- Automated E2E test execution
- Browser installation
- Dev server startup
- Screenshot capture on failure
- Trace collection for debugging

---

## Test Statistics

### Unit & Integration Tests
| Category | Files | Test Cases |
|----------|-------|------------|
| API Tests | 3 | 133 |
| UI Component Tests | 3 | 45+ |
| Utility Tests | 1 | 8 |
| **Total** | **7** | **186+** |

### E2E Tests
| File | Scenarios |
|------|-----------|
| Homepage | 5 |
| Shopping Cart | 10 |
| Product Details | 4 |
| **Total** | **19** |

### Overall Test Count: **205+ Tests**

---

## Running Tests

### Unit Tests
```bash
# Watch mode
npm test

# Single run
npm run test:run

# With coverage
npm run test:coverage

# Interactive UI
npm run test:ui
```

### E2E Tests
```bash
# Headless mode
npm run test:e2e

# Interactive UI
npm run test:e2e:ui

# Headed mode (visible browser)
npm run test:e2e:headed

# Debug mode
npm run test:e2e:debug
```

---

## Key Dependencies

### Testing Dependencies
```json
{
  "@playwright/test": "^1.56.1",
  "@testing-library/jest-dom": "^6.9.1",
  "@testing-library/react": "^16.3.0",
  "@testing-library/user-event": "^14.6.1",
  "@vitest/coverage-v8": "^4.0.6",
  "@vitest/ui": "^4.0.6",
  "axios-mock-adapter": "^2.1.0",
  "jsdom": "^27.1.0",
  "vitest": "^4.0.6"
}
```

---

## Test Quality Metrics

### ✅ Strengths
- Comprehensive API testing with error scenarios
- Full coverage of UI component variants
- User flow validation through E2E tests
- Mock data factory pattern for flexibility
- Proper test isolation with beforeEach/afterEach
- Accessibility-focused queries (getByRole)

### 🎯 Testing Philosophy
1. **Test behavior, not implementation**
2. **Cover happy paths and edge cases**
3. **Simulate real user interactions**
4. **Maintain test independence**
5. **Use descriptive test names**

---

## Future Improvements

### Potential Enhancements
- [ ] Add visual regression testing (Playwright screenshots)
- [ ] Implement performance testing (Lighthouse CI)
- [ ] Add accessibility testing (axe-core)
- [ ] Create integration tests for React context
- [ ] Add mutation testing (Stryker)
- [ ] Implement API contract testing
- [ ] Add load testing for E2E scenarios

---

## Conclusion

The shop-frontend testing suite provides robust coverage across all layers of the application:
- **Unit tests** ensure individual components and utilities work correctly
- **Integration tests** verify API client behavior with mocked backends
- **E2E tests** validate complete user journeys

The testing infrastructure is well-configured with modern tools (Vitest, Playwright) and follows industry best practices for maintainability and reliability.

**Total Test Coverage**: 205+ tests across unit, integration, and E2E layers.

---

*Last Updated: November 5, 2025*
