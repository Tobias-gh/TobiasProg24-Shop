## 📊 Summary 

### Backend
- **Code coverage:** 87%
- **Unit tests:**
  - `Shop.Api.Tests`: 131 tests
  - `Shop.Application.Tests`: 70 tests
  - `Shop.Domain.Tests`: 1 test
  - **Total backend unit tests:** 202
- **Integration Tests:** 42 tests

### Frontend
- **Code coverage:** 100%
- **Unit tests:** 125 tests
- **E2E tests:** 36 tests

### 📈 Total Test Count
- **Backend:** 244 tests (202 unit + 42 integration)
- **Frontend:** 161 tests (125 unit + 36 E2E)
- **Grand Total:** 405 tests ✅

---


## Backend 

## Getting Started
	To see code coverage results use the `./run-coverage.ps1` script.
	To activate the test-watcher use the `./run-tests-watcher.ps1` script.
	

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**	
2. **Install dependencies**
3. **Configure database connection**
Update `src/server/Shop.Api/appsettings.json`:
4. **Create database and run migrations**
5. **Run the application**


The API will be available at:
- HTTP: `http://localhost:5064`
- Swagger: `http://localhost:5064/swagger`

---

## 📡 API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get all products |
| `GET` | `/api/products/{id}` | Get product by ID |
| `POST` | `/api/products` | Create new product |
| `PUT` | `/api/products/{id}` | Update product |
| `DELETE` | `/api/products/{id}` | Delete product |

### Categories

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/categories` | Get all categories |
| `GET` | `/api/categories/{id}` | Get category by ID |

### Shopping Cart

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/carts/{sessionId}` | Get cart by session ID |
| `POST` | `/api/carts/{sessionId}/items` | Add item to cart |
| `PUT` | `/api/carts/{sessionId}/items/{itemId}` | Update cart item quantity |
| `DELETE` | `/api/carts/{sessionId}/items/{itemId}` | Remove item from cart |
| `DELETE` | `/api/carts/{sessionId}` | Clear entire cart |

# Shop Frontend

A modern e-commerce frontend application built with React, TypeScript, and Vite.

## 🚀 Features

- **Product Browsing**: View products in a responsive grid layout
- **Product Details**: Detailed product information pages
- **Shopping Cart**: Add, update, and remove items from cart
- **Category Filtering**: Filter products by categories
- **Responsive Design**: Mobile-friendly UI with Tailwind CSS

## 🛠️ Tech Stack

- **React 18** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Styling
- **Axios** - HTTP client
- **React Router** - Navigation
- **Vitest** - Unit testing
- **Playwright** - E2E testing

## 📋 Prerequisites

- **Node.js** 18+ 
- **npm** 9+
- **Backend API** running on `http://localhost:5064`

## 🔧 Installation

1. Clone the repository
2. Install dependencies:

```bash
npm install
```

## 🏃 Running the Application

### Development Mode

```bash
npm run dev
```

The application will start at `http://localhost:5173`

### Production Build

```bash
npm run build
npm run preview
```

## 🧪 Testing

### Unit & Integration Tests

```bash
# Run tests in watch mode
npm test

# Run tests once
npm run test:run

# Run tests with coverage
npm run test:coverage

# Open test UI
npm run test:ui
```

### End-to-End Tests

**Note**: Make sure the backend is running before running E2E tests.

```bash
# Run E2E tests (headless)
npm run test:e2e

# Run E2E tests with UI
npm run test:e2e:ui

# Run E2E tests in headed mode (visible browser)
npm run test:e2e:headed

# Debug E2E tests
npm run test:e2e:debug

# Browser report for E2E tests
start coverage/index.html
```
