import { apiClient } from './apiClient';
import type { Product } from '../types';

export const productsApi = {
  // Get all products
  getAll: async (): Promise<Product[]> => {
    const response = await apiClient.get<Product[]>('/api/products');
    return response.data;
  },

  // Get product by ID
  getById: async (id: string): Promise<Product> => {
    const response = await apiClient.get<Product>(`/api/products/${id}`);
    return response.data;
  },

  // Get products by category
  getByCategory: async (categoryId: string): Promise<Product[]> => {
    const response = await apiClient.get<Product[]>('/api/products', {
      params: { categoryId },
    });
    return response.data;
  },
};
