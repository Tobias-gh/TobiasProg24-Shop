import { test, expect } from '@playwright/test';

test.describe('Homepage', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
  });

  test('should load the homepage successfully', async ({ page }) => {
    await expect(page.getByTestId('home-page')).toBeVisible();
    await expect(page.getByTestId('page-title')).toHaveText('Welcome to Our Shop');
    await expect(page.getByTestId('page-subtitle')).toContainText('Browse our collection');
  });

  test('should display the header with navigation', async ({ page }) => {
    await expect(page.getByTestId('header')).toBeVisible();
    await expect(page.getByTestId('logo-link')).toBeVisible();
    await expect(page.getByTestId('home-link')).toBeVisible();
    await expect(page.getByTestId('cart-link')).toBeVisible();
  });

  test('should display products in a grid', async ({ page }) => {
    // Wait for products to load
    await expect(page.getByTestId('product-list')).toBeVisible();
    
    // Check that product cards are displayed
    const productCards = page.getByTestId('product-card');
    await expect(productCards.first()).toBeVisible();
    
    // Verify at least one product card has all expected elements
    const firstCard = productCards.first();
    await expect(firstCard.getByTestId('product-title')).toBeVisible();
    await expect(firstCard.getByTestId('product-price')).toBeVisible();
    await expect(firstCard.getByTestId('add-to-cart-button')).toBeVisible();
  });

  test('should navigate to product details when clicking View Details', async ({ page }) => {
    await expect(page.getByTestId('product-list')).toBeVisible();
    
    // Click the first "View Details" button
    const firstViewDetailsButton = page.getByTestId('view-details-button').first();
    await firstViewDetailsButton.click();
    
    // Should navigate to a product detail page
    await expect(page).toHaveURL(/\/product\/.+/);
  });

  test('should navigate to cart page when clicking cart link', async ({ page }) => {
    await page.getByTestId('cart-link').click();
    await expect(page).toHaveURL('/cart');
    await expect(page.getByTestId('cart-page')).toBeVisible();
  });
});
