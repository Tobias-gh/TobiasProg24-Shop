import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import MockAdapter from 'axios-mock-adapter';
import { apiClient } from '../../src/api/apiClient';
import { categoriesApi } from '../../src/api/categories.api';
import { mockCategories } from '../helpers/mockData';

describe('categoriesApi', () => {
  let mock: MockAdapter;

  beforeEach(() => {
    mock = new MockAdapter(apiClient);
  });

  afterEach(() => {
    mock.restore();
  });

  describe('getAll', () => {
    it('should fetch all categories successfully', async () => {
      // Arrange
      mock.onGet('/api/categories').reply(200, mockCategories);

      // Act
      const result = await categoriesApi.getAll();

      // Assert
      expect(result).toEqual(mockCategories);
      expect(result).toHaveLength(3);
    });

    it('should return empty array when no categories exist', async () => {
      // Arrange
      mock.onGet('/api/categories').reply(200, []);

      // Act
      const result = await categoriesApi.getAll();

      // Assert
      expect(result).toEqual([]);
      expect(result).toHaveLength(0);
    });

    it('should include productCount in response', async () => {
      // Arrange
      mock.onGet('/api/categories').reply(200, mockCategories);

      // Act
      const result = await categoriesApi.getAll();

      // Assert
      expect(result[0]).toHaveProperty('productCount');
      expect(result[0].productCount).toBeGreaterThanOrEqual(0);
    });

    it('should handle 500 server error', async () => {
      // Arrange
      mock.onGet('/api/categories').reply(500, { message: 'Server error' });

      // Act & Assert
      await expect(categoriesApi.getAll()).rejects.toThrow('Request failed with status code 500');
    });

    it('should handle network error', async () => {
      // Arrange
      mock.onGet('/api/categories').networkError();

      // Act & Assert
      await expect(categoriesApi.getAll()).rejects.toThrow('Network Error');
    });

    it('should call correct endpoint', async () => {
      // Arrange
      mock.onGet('/api/categories').reply(200, mockCategories);

      // Act
      await categoriesApi.getAll();

      // Assert
      expect(mock.history.get).toHaveLength(1);
      expect(mock.history.get[0].url).toBe('/api/categories');
    });

    it('should return categories with all required fields', async () => {
      // Arrange
      mock.onGet('/api/categories').reply(200, mockCategories);

      // Act
      const result = await categoriesApi.getAll();

      // Assert
      result.forEach(category => {
        expect(category).toHaveProperty('id');
        expect(category).toHaveProperty('name');
        expect(category).toHaveProperty('description');
        expect(category).toHaveProperty('productCount');
      });
    });
  });

  describe('getById', () => {
    const categoryId = 'cat-1';
    const category = mockCategories[0];

    it('should fetch category by id successfully', async () => {
      // Arrange
      mock.onGet(`/api/categories/${categoryId}`).reply(200, category);

      // Act
      const result = await categoriesApi.getById(categoryId);

      // Assert
      expect(result).toEqual(category);
      expect(result.id).toBe(categoryId);
    });

    it('should handle 404 when category not found', async () => {
      // Arrange
      const invalidId = 'invalid-id';
      mock.onGet(`/api/categories/${invalidId}`).reply(404, { 
        message: `Category with ID ${invalidId} not found` 
      });

      // Act & Assert
      await expect(categoriesApi.getById(invalidId)).rejects.toThrow('Request failed with status code 404');
    });

    it('should call correct endpoint with id', async () => {
      // Arrange
      mock.onGet(`/api/categories/${categoryId}`).reply(200, category);

      // Act
      await categoriesApi.getById(categoryId);

      // Assert
      expect(mock.history.get).toHaveLength(1);
      expect(mock.history.get[0].url).toBe(`/api/categories/${categoryId}`);
    });

    it('should handle empty id', async () => {
      // Arrange
      const emptyId = '';
      mock.onGet(`/api/categories/${emptyId}`).reply(400);

      // Act & Assert
      await expect(categoriesApi.getById(emptyId)).rejects.toThrow();
    });

    it('should handle invalid GUID format', async () => {
      // Arrange
      const invalidGuid = 'not-a-guid';
      mock.onGet(`/api/categories/${invalidGuid}`).reply(400, { 
        message: 'Invalid GUID format' 
      });

      // Act & Assert
      await expect(categoriesApi.getById(invalidGuid)).rejects.toThrow();
    });

    it('should handle server error', async () => {
      // Arrange
      mock.onGet(`/api/categories/${categoryId}`).reply(500);

      // Act & Assert
      await expect(categoriesApi.getById(categoryId)).rejects.toThrow('Request failed with status code 500');
    });

    it('should return category with all required fields', async () => {
      // Arrange
      mock.onGet(`/api/categories/${categoryId}`).reply(200, category);

      // Act
      const result = await categoriesApi.getById(categoryId);

      // Assert
      expect(result).toHaveProperty('id');
      expect(result).toHaveProperty('name');
      expect(result).toHaveProperty('description');
      expect(result).toHaveProperty('productCount');
    });
  });
});