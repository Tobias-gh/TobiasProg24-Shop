// API Response Types
export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: string;
  categoryName: string;
}

export interface Category {
  id: string;
  name: string;
  description: string;
  productCount: number;
}

export interface CartItem {
  id: string;
  productId: string;
  productName: string;
  productDescription: string;
  productPrice: number;
  quantity: number;
  subtotal: number;
  availableStock: number;
  addedAt: string;
}

export interface Cart {
  id: string;
  sessionId: string;
  items: CartItem[];
  totalItems: number;
  totalPrice: number;
  createdAt: string;
  updatedAt: string;
}

// Request Types
export interface AddToCartRequest {
  productId: string;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}
