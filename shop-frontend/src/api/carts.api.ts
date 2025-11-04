import { apiClient } from './apiClient';
import type { Cart, AddToCartRequest, UpdateCartItemRequest } from '../types';

export const cartsApi = {
  // Get cart by session ID
  getCart: async (sessionId: string): Promise<Cart> => {
    const response = await apiClient.get<Cart>(`/api/carts/${sessionId}`);
    return response.data;
  },

  // Add item to cart
  addItem: async (sessionId: string, request: AddToCartRequest): Promise<Cart> => {
    const response = await apiClient.post<Cart>(
      `/api/carts/${sessionId}/items`,
      request
    );
    return response.data;
  },

  // Update cart item quantity
  updateItem: async (
    sessionId: string,
    itemId: string,
    request: UpdateCartItemRequest
  ): Promise<Cart> => {
    const response = await apiClient.put<Cart>(
      `/api/carts/${sessionId}/items/${itemId}`,
      request
    );
    return response.data;
  },

  // Remove item from cart
  removeItem: async (sessionId: string, itemId: string): Promise<Cart> => {
    const response = await apiClient.delete<Cart>(
      `/api/carts/${sessionId}/items/${itemId}`
    );
    return response.data;
  },

  // Clear entire cart
  clearCart: async (sessionId: string): Promise<void> => {
    await apiClient.delete(`/api/carts/${sessionId}`);
  },
};

// Helper to generate or retrieve session ID
export const getSessionId = (): string => {
  let sessionId = localStorage.getItem('cartSessionId');
  if (!sessionId) {
    sessionId = crypto.randomUUID();
    localStorage.setItem('cartSessionId', sessionId);
  }
  return sessionId;
};
