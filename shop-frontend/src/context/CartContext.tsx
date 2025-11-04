import React, { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import type { Cart, AddToCartRequest } from '../types';
import { cartsApi, getSessionId } from '../api';

interface CartContextType {
  cart: Cart | null;
  loading: boolean;
  error: string | null;
  addToCart: (request: AddToCartRequest) => Promise<void>;
  updateQuantity: (itemId: string, quantity: number) => Promise<void>;
  removeFromCart: (itemId: string) => Promise<void>;
  clearCart: () => Promise<void>;
  refreshCart: () => Promise<void>;
}

const CartContext = createContext<CartContextType | undefined>(undefined);

export const CartProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const sessionId = getSessionId();

  const refreshCart = async () => {
    try {
      setLoading(true);
      setError(null);
      const cartData = await cartsApi.getCart(sessionId);
      setCart(cartData);
    } catch (err) {
      // If cart doesn't exist yet, it's okay
      if ((err as any)?.response?.status === 404) {
        setCart(null);
      } else {
        setError('Failed to load cart');
        console.error('Error loading cart:', err);
      }
    } finally {
      setLoading(false);
    }
  };

  const addToCart = async (request: AddToCartRequest) => {
    try {
      setLoading(true);
      setError(null);
      const updatedCart = await cartsApi.addItem(sessionId, request);
      setCart(updatedCart);
    } catch (err) {
      setError('Failed to add item to cart');
      console.error('Error adding to cart:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const updateQuantity = async (itemId: string, quantity: number) => {
    try {
      setLoading(true);
      setError(null);
      const updatedCart = await cartsApi.updateItem(sessionId, itemId, { quantity });
      setCart(updatedCart);
    } catch (err) {
      setError('Failed to update cart item');
      console.error('Error updating cart item:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const removeFromCart = async (itemId: string) => {
    try {
      setLoading(true);
      setError(null);
      const updatedCart = await cartsApi.removeItem(sessionId, itemId);
      setCart(updatedCart);
    } catch (err) {
      setError('Failed to remove item from cart');
      console.error('Error removing from cart:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const clearCart = async () => {
    try {
      setLoading(true);
      setError(null);
      await cartsApi.clearCart(sessionId);
      setCart(null);
    } catch (err) {
      setError('Failed to clear cart');
      console.error('Error clearing cart:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  // Load cart on mount
  useEffect(() => {
    refreshCart();
  }, []);

  return (
    <CartContext.Provider
      value={{
        cart,
        loading,
        error,
        addToCart,
        updateQuantity,
        removeFromCart,
        clearCart,
        refreshCart,
      }}
    >
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => {
  const context = useContext(CartContext);
  if (context === undefined) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
};
