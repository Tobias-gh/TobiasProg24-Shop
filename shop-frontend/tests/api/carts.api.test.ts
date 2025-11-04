import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import MockAdapter from 'axios-mock-adapter';
import { apiClient } from '../../src/api/apiClient';
import { cartsApi, getSessionId } from '../../src/api/carts.api'; // ← ADD getSessionId here
import { mockCart, mockEmptyCart, createMockCartItem } from '../helpers/mockData';
import type { AddToCartRequest, UpdateCartItemRequest } from '../../src/types';

describe('cartsApi', () => {
  let mock: MockAdapter;
  const sessionId = 'test-session-123';

  beforeEach(() => {
    mock = new MockAdapter(apiClient);
  });

  afterEach(() => {
    mock.restore();
  });

  describe('getCart', () => {
    it('should fetch cart by session id successfully', async () => {
      // Arrange
      mock.onGet(`/api/carts/${sessionId}`).reply(200, mockCart);

      // Act
      const result = await cartsApi.getCart(sessionId);

      // Assert
      expect(result).toEqual(mockCart);
      expect(result.sessionId).toBe(sessionId);
    });

    it('should return empty cart when cart not found', async () => {
      // Arrange
      const newSessionId = 'new-session';
      mock.onGet(`/api/carts/${newSessionId}`).reply(200, mockEmptyCart);

      // Act
      const result = await cartsApi.getCart(newSessionId);

      // Assert
      expect(result.items).toHaveLength(0);
      expect(result.totalItems).toBe(0);
      expect(result.totalPrice).toBe(0);
    });

    it('should handle 404 error gracefully', async () => {
      // Arrange
      mock.onGet(`/api/carts/${sessionId}`).reply(404);

      // Act & Assert
      await expect(cartsApi.getCart(sessionId)).rejects.toThrow();
    });

    it('should call correct endpoint with session id', async () => {
      // Arrange
      mock.onGet(`/api/carts/${sessionId}`).reply(200, mockCart);

      // Act
      await cartsApi.getCart(sessionId);

      // Assert
      expect(mock.history.get).toHaveLength(1);
      expect(mock.history.get[0].url).toBe(`/api/carts/${sessionId}`);
    });

    it('should handle server error', async () => {
      // Arrange
      mock.onGet(`/api/carts/${sessionId}`).reply(500);

      // Act & Assert
      await expect(cartsApi.getCart(sessionId)).rejects.toThrow();
    });
  });

  describe('addItem', () => {
    const request: AddToCartRequest = {
      productId: '1',
      quantity: 2,
    };
    const updatedCart = {
      ...mockCart,
      items: [...mockCart.items, createMockCartItem({ productId: request.productId, quantity: request.quantity })],
    };

    it('should add item to cart successfully', async () => {
      // Arrange
      mock.onPost(`/api/carts/${sessionId}/items`).reply(200, updatedCart);

      // Act
      const result = await cartsApi.addItem(sessionId, request);

      // Assert
      expect(result).toEqual(updatedCart);
      expect(result.items.length).toBeGreaterThan(mockCart.items.length);
    });

    it('should send AddToCartRequest in request body', async () => {
      // Arrange
      mock.onPost(`/api/carts/${sessionId}/items`).reply(200, updatedCart);

      // Act
      await cartsApi.addItem(sessionId, request);

      // Assert
      expect(mock.history.post).toHaveLength(1);
      const requestData = JSON.parse(mock.history.post[0].data);
      expect(requestData).toEqual(request);
    });

    it('should handle 400 when product out of stock', async () => {
      // Arrange
      mock.onPost(`/api/carts/${sessionId}/items`).reply(400, { 
        message: 'Product out of stock' 
      });

      // Act & Assert
      await expect(cartsApi.addItem(sessionId, { productId: '1', quantity: 100 })).rejects.toThrow();
    });

    it('should handle 404 when product not found', async () => {
      // Arrange
      const invalidRequest: AddToCartRequest = { productId: 'invalid-id', quantity: 1 };
      mock.onPost(`/api/carts/${sessionId}/items`).reply(404, { message: 'Product not found' });

      // Act & Assert
      await expect(cartsApi.addItem(sessionId, invalidRequest)).rejects.toThrow();
    });

    it('should handle negative quantity', async () => {
      // Arrange
      const invalidRequest: AddToCartRequest = { productId: '1', quantity: -1 };
      mock.onPost(`/api/carts/${sessionId}/items`).reply(400, { 
        message: 'Quantity must be positive' 
      });

      // Act & Assert
      await expect(cartsApi.addItem(sessionId, invalidRequest)).rejects.toThrow();
    });

    it('should handle zero quantity', async () => {
      // Arrange
      const invalidRequest: AddToCartRequest = { productId: '1', quantity: 0 };
      mock.onPost(`/api/carts/${sessionId}/items`).reply(400, { 
        message: 'Quantity must be at least 1' 
      });

      // Act & Assert
      await expect(cartsApi.addItem(sessionId, invalidRequest)).rejects.toThrow();
    });

    it('should call correct endpoint', async () => {
      // Arrange
      mock.onPost(`/api/carts/${sessionId}/items`).reply(200, updatedCart);

      // Act
      await cartsApi.addItem(sessionId, request);

      // Assert
      expect(mock.history.post[0].url).toBe(`/api/carts/${sessionId}/items`);
    });

    // ✅ Add these to test error branches
    it('should handle 400 bad request error', async () => {
      const request: AddToCartRequest = { productId: '1', quantity: 1 };
      mock.onPost(`/api/carts/${sessionId}/items`).reply(400, { 
        message: 'Invalid request' 
      });

      await expect(cartsApi.addItem(sessionId, request)).rejects.toThrow();
    });

    it('should handle 500 server error', async () => {
      const request: AddToCartRequest = { productId: '1', quantity: 1 };
      mock.onPost(`/api/carts/${sessionId}/items`).reply(500);

      await expect(cartsApi.addItem(sessionId, request)).rejects.toThrow();
    });

    it('should handle network timeout', async () => {
      const request: AddToCartRequest = { productId: '1', quantity: 1 };
      mock.onPost(`/api/carts/${sessionId}/items`).timeout();

      await expect(cartsApi.addItem(sessionId, request)).rejects.toThrow();
    });
  });

  describe('updateItem', () => {
    const itemId = 'cart-item-1';
    const request: UpdateCartItemRequest = { quantity: 3 };
    const updatedCart = {
      ...mockCart,
      items: mockCart.items.map(item => 
        item.id === itemId ? { ...item, quantity: request.quantity } : item
      ),
    };

    it('should update item quantity successfully', async () => {
      // Arrange
      mock.onPut(`/api/carts/${sessionId}/items/${itemId}`).reply(200, updatedCart);

      // Act
      const result = await cartsApi.updateItem(sessionId, itemId, request);

      // Assert
      expect(result).toEqual(updatedCart);
      const updatedItem = result.items.find(i => i.id === itemId);
      expect(updatedItem?.quantity).toBe(request.quantity);
    });

    it('should send UpdateCartItemRequest in request body', async () => {
      // Arrange
      mock.onPut(`/api/carts/${sessionId}/items/${itemId}`).reply(200, updatedCart);

      // Act
      await cartsApi.updateItem(sessionId, itemId, request);

      // Assert
      expect(mock.history.put).toHaveLength(1);
      const requestData = JSON.parse(mock.history.put[0].data);
      expect(requestData).toEqual(request);
    });

    it('should handle 404 when item not found', async () => {
      // Arrange
      const invalidItemId = 'invalid-item-id';
      mock.onPut(`/api/carts/${sessionId}/items/${invalidItemId}`).reply(404, 'Cart item not found');

      // Act & Assert
      await expect(cartsApi.updateItem(sessionId, invalidItemId, request)).rejects.toThrow();
    });

    it('should handle quantity exceeding stock', async () => {
      // Arrange
      mock.onPut(`/api/carts/${sessionId}/items/${itemId}`).reply(400, 'Quantity exceeds available stock');

      // Act & Assert
      await expect(cartsApi.updateItem(sessionId, itemId, { quantity: 1000 })).rejects.toThrow();
    });

    it('should handle updating to zero quantity', async () => {
      // Arrange
      mock.onPut(`/api/carts/${sessionId}/items/${itemId}`).reply(400, 'Quantity must be at least 1');

      // Act & Assert
      await expect(cartsApi.updateItem(sessionId, itemId, { quantity: 0 })).rejects.toThrow();
    });

    it('should handle negative quantity', async () => {
      // Arrange
      mock.onPut(`/api/carts/${sessionId}/items/${itemId}`).reply(400, 'Quantity must be positive');

      // Act & Assert
      await expect(cartsApi.updateItem(sessionId, itemId, { quantity: -5 })).rejects.toThrow();
    });

    it('should call correct endpoint', async () => {
      // Arrange
      mock.onPut(`/api/carts/${sessionId}/items/${itemId}`).reply(200, updatedCart);

      // Act
      await cartsApi.updateItem(sessionId, itemId, request);

      // Assert
      expect(mock.history.put[0].url).toBe(`/api/carts/${sessionId}/items/${itemId}`);
    });
  });

  describe('removeItem', () => {
    const itemId = 'cart-item-1';
    const updatedCart = {
      ...mockCart,
      items: mockCart.items.filter(item => item.id !== itemId),
    };

    it('should remove item from cart successfully', async () => {
      // Arrange
      mock.onDelete(`/api/carts/${sessionId}/items/${itemId}`).reply(200, updatedCart);

      // Act
      const result = await cartsApi.removeItem(sessionId, itemId);

      // Assert
      expect(result).toEqual(updatedCart);
      expect(result.items.find(i => i.id === itemId)).toBeUndefined();
    });

    it('should call correct endpoint with DELETE method', async () => {
      // Arrange
      mock.onDelete(`/api/carts/${sessionId}/items/${itemId}`).reply(200, updatedCart);

      // Act
      await cartsApi.removeItem(sessionId, itemId);

      // Assert
      expect(mock.history.delete).toHaveLength(1);
      expect(mock.history.delete[0].url).toBe(`/api/carts/${sessionId}/items/${itemId}`);
    });

    it('should handle 404 when item not found', async () => {
      // Arrange
      const invalidItemId = 'invalid-item-id';
      mock.onDelete(`/api/carts/${sessionId}/items/${invalidItemId}`).reply(404, 'Cart item not found');

      // Act & Assert
      await expect(cartsApi.removeItem(sessionId, invalidItemId)).rejects.toThrow();
    });

    it('should handle removing last item from cart', async () => {
      // Arrange
      const emptyCart = { ...mockCart, items: [], totalItems: 0, totalPrice: 0 };
      mock.onDelete(`/api/carts/${sessionId}/items/${itemId}`).reply(200, emptyCart);

      // Act
      const result = await cartsApi.removeItem(sessionId, itemId);

      // Assert
      expect(result.items).toHaveLength(0);
      expect(result.totalItems).toBe(0);
    });

    it('should handle server error', async () => {
      // Arrange
      mock.onDelete(`/api/carts/${sessionId}/items/${itemId}`).reply(500);

      // Act & Assert
      await expect(cartsApi.removeItem(sessionId, itemId)).rejects.toThrow();
    });
  });

  describe('clearCart', () => {
    it('should clear all items from cart successfully', async () => {
      // Arrange
      mock.onDelete(`/api/carts/${sessionId}`).reply(204); // NoContent

      // Act
      await cartsApi.clearCart(sessionId);

      // Assert
      expect(mock.history.delete).toHaveLength(1);
      expect(mock.history.delete[0].url).toBe(`/api/carts/${sessionId}`);
    });

    it('should call correct endpoint', async () => {
      // Arrange
      mock.onDelete(`/api/carts/${sessionId}`).reply(204);

      // Act
      await cartsApi.clearCart(sessionId);

      // Assert
      expect(mock.history.delete[0].url).toBe(`/api/carts/${sessionId}`);
    });

    it('should handle 404 when cart not found', async () => {
      // Arrange
      const invalidSessionId = 'invalid-session';
      mock.onDelete(`/api/carts/${invalidSessionId}`).reply(404, 'Cart not found');

      // Act & Assert
      await expect(cartsApi.clearCart(invalidSessionId)).rejects.toThrow();
    });

    it('should handle server error', async () => {
      // Arrange
      mock.onDelete(`/api/carts/${sessionId}`).reply(500);

      // Act & Assert
      await expect(cartsApi.clearCart(sessionId)).rejects.toThrow();
    });

    it('should return void on success', async () => {
      // Arrange
      mock.onDelete(`/api/carts/${sessionId}`).reply(204);

      // Act
      const result = await cartsApi.clearCart(sessionId);

      // Assert
      expect(result).toBeUndefined(); // void returns undefined
    });
  });
});

// ✅ ADD THIS NEW DESCRIBE BLOCK OUTSIDE OF cartsApi describe
describe('getSessionId', () => {
  beforeEach(() => {
    // Clear localStorage before each test
    localStorage.clear();
    vi.clearAllMocks();
  });

  afterEach(() => {
    // Clean up after each test
    localStorage.clear();
  });

  it('should generate new session id if none exists', () => {
    // Arrange
    expect(localStorage.getItem('cartSessionId')).toBeNull();

    // Act
    const sessionId = getSessionId();

    // Assert
    expect(sessionId).toBeDefined();
    expect(typeof sessionId).toBe('string');
    expect(sessionId.length).toBeGreaterThan(0);
  });

  it('should store session id in localStorage', () => {
    // Arrange
    localStorage.clear();

    // Act
    const sessionId = getSessionId();

    // Assert
    const storedId = localStorage.getItem('cartSessionId');
    expect(storedId).toBe(sessionId);
    expect(storedId).toBeDefined();
    expect(storedId).not.toBe('');
  });

  it('should return existing session id from localStorage', () => {
    // Arrange
    localStorage.clear();
    const existingSessionId = 'test-session-abc-123';
    localStorage.setItem('cartSessionId', existingSessionId);
    
    // Verify it was set
    expect(localStorage.getItem('cartSessionId')).toBe(existingSessionId);

    // Act
    const sessionId = getSessionId();

    // Assert
    expect(sessionId).toBe(existingSessionId);
    // Should not have changed it
    expect(localStorage.getItem('cartSessionId')).toBe(existingSessionId);
  });

  it('should return same session id on multiple calls', () => {
    // Arrange
    localStorage.clear();

    // Act
    const sessionId1 = getSessionId();
    const sessionId2 = getSessionId();
    const sessionId3 = getSessionId();

    // Assert
    expect(sessionId1).toBe(sessionId2);
    expect(sessionId2).toBe(sessionId3);
  });

  it('should generate unique session ids for different sessions', () => {
    // Arrange & Act
    const sessionId1 = getSessionId();
    
    // Simulate new session
    localStorage.clear();
    
    const sessionId2 = getSessionId();

    // Assert
    expect(sessionId1).not.toBe(sessionId2);
    expect(sessionId1).toBeDefined();
    expect(sessionId2).toBeDefined();
  });

  it('should use existing valid session id', () => {
    // Arrange
    localStorage.clear();
    const validSessionId = 'valid-uuid-12345';
    localStorage.setItem('cartSessionId', validSessionId);

    // Act
    const sessionId = getSessionId();

    // Assert
    expect(sessionId).toBe(validSessionId);
    expect(localStorage.getItem('cartSessionId')).toBe(validSessionId);
  });
});