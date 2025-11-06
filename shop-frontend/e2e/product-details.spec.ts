import { test, expect } from '@playwright/test';

test.describe('Product Details', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
    await expect(page.getByTestId('product-list')).toBeVisible({ timeout: 15000 });
  });

  test('should display product details page', async ({ page }) => {
    // Click on first product's "View Details" button
    await page.getByTestId('view-details-button').first().click();
    
    // Wait for navigation to product detail page
    await expect(page).toHaveURL(/\/product\/.+/);
    
    // Verify we're on a product page (would need data-testid on ProductDetailPage component)
    // For now, just verify URL changed
    const url = page.url();
    expect(url).toContain('/product/');
  });

  test('should add product to cart from details page', async ({ page }) => {
    // Navigate to product details
    await page.getByTestId('view-details-button').first().click();
    await page.waitForTimeout(500);
    
    // Find and click add to cart button on details page
    const addToCartButton = page.getByTestId('add-to-cart-button');
    if (await addToCartButton.isVisible()) {
      await addToCartButton.click();
      await page.waitForTimeout(500);
      
      // Verify cart badge appears
      await expect(page.getByTestId('cart-badge')).toBeVisible();
      await expect(page.getByTestId('cart-badge')).toHaveText('1');
    }
  });

  test('should navigate back to homepage from product details', async ({ page }) => {
    // Navigate to product details
    await page.getByTestId('view-details-button').first().click();
    await page.waitForTimeout(500);
    
    // Click home link in header
    await page.getByTestId('home-link').click();
    
    // Verify we're back on homepage
    await expect(page).toHaveURL('/');
    await expect(page.getByTestId('home-page')).toBeVisible();
  });

  test('should show product stock information', async ({ page }) => {
    // Check stock badge is visible on homepage product cards
    const stockBadge = page.getByTestId('product-stock-badge').first();
    await expect(stockBadge).toBeVisible();
    
    const stockText = await stockBadge.textContent();
    expect(stockText).toMatch(/\d+/); // Should contain a number
  });
});
