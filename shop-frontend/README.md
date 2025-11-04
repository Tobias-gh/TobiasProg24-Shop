# Shop Frontend

A modern e-commerce frontend built with React, TypeScript, Tailwind CSS, and shadcn/ui.

## 🚀 Features

- **Product Browsing**: View all products with filtering by category
- **Product Details**: Detailed view of individual products
- **Shopping Cart**: Add/remove items, update quantities, view cart total
- **Category Navigation**: Filter products by category
- **Responsive Design**: Mobile-friendly UI with Tailwind CSS
- **Modern UI Components**: Using shadcn/ui for consistent design

## 🛠️ Tech Stack

- **React 19** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool
- **React Router** - Client-side routing
- **Axios** - HTTP client
- **Tailwind CSS** - Styling
- **shadcn/ui** - UI components
- **Lucide React** - Icons

## 📁 Project Structure

```
src/
├── api/                    # API client and service layers
│   ├── apiClient.ts       # Axios configuration
│   ├── products.api.ts    # Products endpoints
│   ├── categories.api.ts  # Categories endpoints
│   └── carts.api.ts       # Cart endpoints
├── components/            # Reusable components
│   ├── ui/               # shadcn/ui base components
│   ├── Header/           # Navigation header
│   ├── ProductCard/      # Product card component
│   ├── ProductList/      # Products grid
│   ├── CategoryFilter/   # Category filter buttons
│   └── Cart/             # Shopping cart component
├── context/              # React Context providers
│   └── CartContext.tsx   # Cart state management
├── pages/                # Page components
│   ├── HomePage/         # Main products page
│   ├── ProductDetailPage/ # Product details
│   └── CartPage/         # Cart page
├── types/                # TypeScript type definitions
└── lib/                  # Utility functions
```

## 🔧 Configuration

### API Base URL

The API base URL is configured in `src/api/apiClient.ts`:

```typescript
const API_BASE_URL = 'http://localhost:5000';
```

**⚠️ Important:** Change this to match your backend API URL if it's different.

## 🚀 Getting Started

### Prerequisites

- Node.js 18+ installed
- Your backend API running (default: http://localhost:5000)

### Installation

1. Install dependencies:
```bash
npm install
```

2. Start the development server:
```bash
npm run dev
```

The app will be available at `http://localhost:5173`

### Build for Production

```bash
npm run build
```

The production-ready files will be in the `dist/` directory.

## 🔌 API Integration

The frontend expects the following API endpoints:

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/categories` - Get all categories
- `GET /api/carts/{sessionId}` - Get cart by session ID
- `POST /api/carts/{sessionId}/items` - Add item to cart
- `PUT /api/carts/{sessionId}/items/{itemId}` - Update cart item
- `DELETE /api/carts/{sessionId}/items/{itemId}` - Remove cart item
- `DELETE /api/carts/{sessionId}` - Clear cart

### Cart Session Management

The frontend automatically generates a unique session ID stored in localStorage to track the user's cart across sessions.

## 🐛 Troubleshooting

### API Connection Issues

1. Make sure your backend API is running
2. Check the API base URL in `src/api/apiClient.ts`
3. Verify CORS is enabled on your backend for `http://localhost:5173`

