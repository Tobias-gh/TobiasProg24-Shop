import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import MockAdapter from 'axios-mock-adapter';
import { apiClient } from '../../src/api/apiClient'; // ← Fixed
import { productsApi } from '../../src/api/products.api'; // ← Fixed
import { mockProducts } from '../helpers/mockData'; // ← Fixed
describe('productsApi', () => {
  let mock: MockAdapter;

  beforeEach(() => {
    mock = new MockAdapter(apiClient);
  });

  afterEach(() => {
    mock.restore();
  });

  describe('getAll', () => {
    it('should fetch all products successfully', async () => {
      // Arrange
      mock.onGet('/api/products').reply(200, mockProducts);

      // Act
      const result = await productsApi.getAll();

      // Assert
      expect(result).toEqual(mockProducts);
      expect(result).toHaveLength(mockProducts.length);
    });

    it('should return empty array when no products exist', async () => {
      // Arrange
      mock.onGet('/api/products').reply(200, []);

      // Act
      const result = await productsApi.getAll();

      // Assert
      expect(result).toEqual([]);
      expect(result).toHaveLength(0);
    });

    it('should handle 500 server error', async () => {
      // Arrange
      mock.onGet('/api/products').reply(500, { message: 'Internal Server Error' });

      // Act & Assert
      await expect(productsApi.getAll()).rejects.toThrow('Request failed with status code 500');
    });

    it('should handle network error', async () => {
      // Arrange
      mock.onGet('/api/products').networkError();

      // Act & Assert
      await expect(productsApi.getAll()).rejects.toThrow('Network Error');
    });

    it('should call correct endpoint', async () => {
      // Arrange
      mock.onGet('/api/products').reply(200, mockProducts);

      // Act
      await productsApi.getAll();

      // Assert
      expect(mock.history.get).toHaveLength(1);
      expect(mock.history.get[0].url).toBe('/api/products');
    });

    it('should return products with all required fields', async () => {
      // Arrange
      mock.onGet('/api/products').reply(200, mockProducts);

      // Act
      const result = await productsApi.getAll();

      // Assert
      result.forEach(product => {
        expect(product).toHaveProperty('id');
        expect(product).toHaveProperty('name');
        expect(product).toHaveProperty('description');
        expect(product).toHaveProperty('price');
        expect(product).toHaveProperty('stock');
        expect(product).toHaveProperty('categoryId');
        expect(product).toHaveProperty('categoryName');
      });
    });

    it('should return products with correct data types', async () => {
      // Arrange
      mock.onGet('/api/products').reply(200, mockProducts);

      // Act
      const result = await productsApi.getAll();

      // Assert
      result.forEach(product => {
        expect(typeof product.id).toBe('string');
        expect(typeof product.name).toBe('string');
        expect(typeof product.price).toBe('number');
        expect(typeof product.stock).toBe('number');
      });
    });
  });

  describe('getById', () => {
    const productId = '1';
    const product = mockProducts[0];

    it('should fetch product by id successfully', async () => {
      // Arrange
      mock.onGet(`/api/products/${productId}`).reply(200, product);

      // Act
      const result = await productsApi.getById(productId);

      // Assert
      expect(result).toEqual(product);
      expect(result.id).toBe(productId);
    });

    it('should handle 404 when product not found', async () => {
      // Arrange
      const invalidId = 'invalid-id';
      mock.onGet(`/api/products/${invalidId}`).reply(404, { 
        message: `product with ID ${invalidId} not found` 
      });

      // Act & Assert
      await expect(productsApi.getById(invalidId)).rejects.toThrow('Request failed with status code 404');
    });

    it('should call correct endpoint with id', async () => {
      // Arrange
      mock.onGet(`/api/products/${productId}`).reply(200, product);

      // Act
      await productsApi.getById(productId);

      // Assert
      expect(mock.history.get).toHaveLength(1);
      expect(mock.history.get[0].url).toBe(`/api/products/${productId}`);
    });

    it('should handle empty string id', async () => {
      // Arrange
      mock.onGet('/api/products/').reply(400, { message: 'Bad Request' });

      // Act & Assert
      await expect(productsApi.getById('')).rejects.toThrow();
    });

    it('should handle invalid GUID format', async () => {
      // Arrange
      const invalidGuid = 'not-a-guid';
      mock.onGet(`/api/products/${invalidGuid}`).reply(400, { 
        message: 'Invalid GUID format' 
      });

      // Act & Assert
      await expect(productsApi.getById(invalidGuid)).rejects.toThrow();
    });

    it('should handle server error', async () => {
      // Arrange
      mock.onGet(`/api/products/${productId}`).reply(500);

      // Act & Assert
      await expect(productsApi.getById(productId)).rejects.toThrow('Request failed with status code 500');
    });

    it('should return product with all required fields', async () => {
      // Arrange
      mock.onGet(`/api/products/${productId}`).reply(200, product);

      // Act
      const result = await productsApi.getById(productId);

      // Assert
      expect(result).toHaveProperty('id');
      expect(result).toHaveProperty('name');
      expect(result).toHaveProperty('description');
      expect(result).toHaveProperty('price');
      expect(result).toHaveProperty('stock');
      expect(result).toHaveProperty('categoryId');
      expect(result).toHaveProperty('categoryName');
    });

    it('should handle product with zero stock', async () => {
      // Arrange
      const outOfStockProduct = { ...product, stock: 0 };
      mock.onGet(`/api/products/${productId}`).reply(200, outOfStockProduct);

      // Act
      const result = await productsApi.getById(productId);

      // Assert
      expect(result.stock).toBe(0);
    });
  });

  describe('getByCategory', () => {
    const categoryId = 'cat-1';
    const electronicsProducts = mockProducts.filter(p => p.categoryId === categoryId);

    it('should fetch products by category successfully', async () => {
      // Arrange
      mock.onGet('/api/products', { params: { categoryId } }).reply(200, electronicsProducts);

      // Act
      const result = await productsApi.getByCategory(categoryId);

      // Assert
      expect(result).toEqual(electronicsProducts);
      expect(result.every(p => p.categoryId === categoryId)).toBe(true);
    });

    it('should return empty array when category has no products', async () => {
      // Arrange
      const emptyCategory = 'cat-empty';
      mock.onGet('/api/products', { params: { categoryId: emptyCategory } }).reply(200, []);

      // Act
      const result = await productsApi.getByCategory(emptyCategory);

      // Assert
      expect(result).toEqual([]);
      expect(result).toHaveLength(0);
    });

    it('should send categoryId as query parameter', async () => {
      // Arrange
      mock.onGet('/api/products').reply(200, electronicsProducts);

      // Act
      await productsApi.getByCategory(categoryId);

      // Assert
      expect(mock.history.get).toHaveLength(1);
      expect(mock.history.get[0].params).toEqual({ categoryId });
    });

    it('should handle server error', async () => {
      // Arrange
      mock.onGet('/api/products', { params: { categoryId } }).reply(500);

      // Act & Assert
      await expect(productsApi.getByCategory(categoryId)).rejects.toThrow();
    });
  });
});