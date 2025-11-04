import type { Product, Category, Cart, CartItem } from '../../src/types';

// Mock Products
export const mockProducts: Product[] = [
  {
    id: '1',
    name: 'Laptop',
    description: 'High-performance laptop for professionals',
    price: 999.99,
    stock: 10,
    categoryId: 'cat-1',
    categoryName: 'Electronics',
  },
  {
    id: '2',
    name: 'Wireless Mouse',
    description: 'Ergonomic wireless mouse with 6 buttons',
    price: 29.99,
    stock: 50,
    categoryId: 'cat-1',
    categoryName: 'Electronics',
  },
  {
    id: '3',
    name: 'Coffee Mug',
    description: 'Ceramic coffee mug, 12oz capacity',
    price: 9.99,
    stock: 0,
    categoryId: 'cat-2',
    categoryName: 'Home & Kitchen',
  },
  {
    id: '4',
    name: 'Notebook',
    description: 'Spiral notebook, 100 pages',
    price: 4.99,
    stock: 100,
    categoryId: 'cat-3',
    categoryName: 'Office Supplies',
  },
];

export const mockCategories: Category[] = [
  {
    id: 'cat-1',
    name: 'Electronics',
    description: 'Electronic devices and accessories',
    productCount: 25,
  },
  {
    id: 'cat-2',
    name: 'Home & Kitchen',
    description: 'Home and kitchen essentials',
    productCount: 15,
  },
  {
    id: 'cat-3',
    name: 'Office Supplies',
    description: 'Office and stationery items',
    productCount: 30,
  },
];

// Helper function to create mock product
export const createMockProduct = (overrides: Partial<Product> = {}): Product => {
  return {
    id: '999',
    name: 'Test Product',
    description: 'Test description',
    price: 99.99,
    stock: 10,
    categoryId: 'cat-1',
    categoryName: 'Test Category',
    ...overrides,
  };
};

// Helper function to create mock cart item
export const createMockCartItem = (overrides: Partial<CartItem> = {}): CartItem => {
  return {
    id: 'cart-item-999',
    productId: '1',
    productName: 'Test Product',
    productDescription: 'Test description',
    productPrice: 99.99,
    quantity: 1,
    subtotal: 99.99,
    availableStock: 10,
    addedAt: new Date().toISOString(),
    ...overrides,
  };
};

// Mock cart with items
export const mockCart: Cart = {
  id: 'cart-123',
  sessionId: 'test-session-123',
  items: [
    {
      id: 'cart-item-1',
      productId: '1',
      productName: 'Laptop',
      productDescription: 'High-performance laptop',
      productPrice: 999.99,
      quantity: 1,
      subtotal: 999.99,
      availableStock: 10,
      addedAt: new Date().toISOString(),
    },
    {
      id: 'cart-item-2',
      productId: '2',
      productName: 'Wireless Mouse',
      productDescription: 'Ergonomic mouse',
      productPrice: 29.99,
      quantity: 2,
      subtotal: 59.98,
      availableStock: 50,
      addedAt: new Date().toISOString(),
    },
  ],
  totalItems: 3,
  totalPrice: 1059.97,
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString(),
};

// Mock empty cart
export const mockEmptyCart: Cart = {
  id: 'cart-empty',
  sessionId: 'empty-session',
  items: [],
  totalItems: 0,
  totalPrice: 0,
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString(),
};