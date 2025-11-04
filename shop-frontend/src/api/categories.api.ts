import { apiClient } from './apiClient';
import type { Category } from '../types';

export const categoriesApi = {
  // Get all categories
  getAll: async (): Promise<Category[]> => {
    const response = await apiClient.get<Category[]>('/api/categories');
    return response.data;
  },

  // Get category by ID
  getById: async (id: string): Promise<Category> => {
    const response = await apiClient.get<Category>(`/api/categories/${id}`);
    return response.data;
  },
};
