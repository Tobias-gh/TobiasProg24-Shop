import { test, expect } from '@playwright/test';

test.describe('Shopping Cart', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
    // Wait for products to load with increased timeout
    await expect(page.getByTestId('product-list')).toBeVisible({ timeout: 15000 });
  });

  test('should show empty cart initially', async ({ page }) => {
    await page.getByTestId('cart-link').click();
    await expect(page.getByTestId('cart-page')).toBeVisible();
    await expect(page.getByTestId('cart-empty')).toBeVisible();
    await expect(page.getByTestId('empty-cart-message')).toHaveText('Your cart is empty');
  });

  test('should add a product to cart from homepage', async ({ page }) => {
    // Add first product to cart
    const firstAddButton = page.getByTestId('add-to-cart-button').first();
    await firstAddButton.click();
    
    // Wait a moment for the cart to update
    await page.waitForTimeout(500);
    
    // Check that cart badge appears with count
    await expect(page.getByTestId('cart-badge')).toBeVisible();
    await expect(page.getByTestId('cart-badge')).toHaveText('1');
  });

  test('should display added product in cart page', async ({ page }) => {
    // Add first product to cart
    await page.getByTestId('add-to-cart-button').first().click();
    await page.waitForTimeout(500);
    
    // Navigate to cart page
    await page.getByTestId('cart-link').click();
    await expect(page.getByTestId('cart-page')).toBeVisible();
    
    // Verify cart is not empty
    await expect(page.getByTestId('cart')).toBeVisible();
    await expect(page.getByTestId('cart-item')).toBeVisible();
    
    // Verify cart item details
    await expect(page.getByTestId('cart-item-name')).toBeVisible();
    await expect(page.getByTestId('cart-item-price')).toBeVisible();
    await expect(page.getByTestId('item-quantity')).toBeVisible();
  });

  test('should increase product quantity in cart', async ({ page }) => {
    // Add product and go to cart
    await page.getByTestId('add-to-cart-button').first().click();
    await page.waitForTimeout(500);
    await page.getByTestId('cart-link').click();
    
    // Get initial quantity
    const quantityElement = page.getByTestId('item-quantity').first();
    const initialQuantity = await quantityElement.textContent();
    
    // Click increase button
    await page.getByTestId('increase-quantity').first().click();
    
    // Wait for quantity to update by checking for the new value
    await expect(quantityElement).toHaveText(String(Number(initialQuantity) + 1), { timeout: 5000 });
    
    // Verify cart badge updated
    await expect(page.getByTestId('cart-badge')).toHaveText('2');
  });

  test('should decrease product quantity in cart', async ({ page }) => {
    // Add product twice and go to cart
    await page.getByTestId('add-to-cart-button').first().click();
    await page.waitForTimeout(500);
    await page.getByTestId('add-to-cart-button').first().click();
    await page.waitForTimeout(500);
    await page.getByTestId('cart-link').click();
    
    // Get initial quantity
    const quantityElement = page.getByTestId('item-quantity').first();
    const initialQuantity = await quantityElement.textContent();
    
    // Click decrease button
    await page.getByTestId('decrease-quantity').first().click();
    
    // Wait for quantity to update by checking for the new value
    await expect(quantityElement).toHaveText(String(Number(initialQuantity) - 1), { timeout: 5000 });
  });

  test('should remove product from cart', async ({ page }) => {
    // Add product and go to cart
    await page.getByTestId('add-to-cart-button').first().click();
    await page.waitForTimeout(500);
    await page.getByTestId('cart-link').click();
    
    // Remove item
    await page.getByTestId('remove-item').first().click();
    await page.waitForTimeout(500);
    
    // Verify cart is empty
    await expect(page.getByTestId('cart-empty')).toBeVisible();
    
    // Verify cart badge is gone
    await expect(page.getByTestId('cart-badge')).not.toBeVisible();
  });

  test('should clear entire cart', async ({ page }) => {
    // Add multiple products
    await page.getByTestId('add-to-cart-button').first().click();
    await page.waitForTimeout(300);
    await page.getByTestId('add-to-cart-button').nth(1).click();
    await page.waitForTimeout(300);
    await page.getByTestId('cart-link').click();
    
    // Clear cart
    await page.getByTestId('clear-cart-button').click();
    await page.waitForTimeout(500);
    
    // Verify cart is empty
    await expect(page.getByTestId('cart-empty')).toBeVisible();
  });

  test('should display correct total price', async ({ page }) => {
    // Add product and go to cart
    await page.getByTestId('add-to-cart-button').first().click();
    await page.waitForTimeout(500);
    await page.getByTestId('cart-link').click();
    
    // Verify total price is displayed
    await expect(page.getByTestId('cart-total-price')).toBeVisible();
    const totalPrice = await page.getByTestId('cart-total-price').textContent();
    expect(totalPrice).toMatch(/\$\d+\.\d{2}/);
  });

  test('should navigate back to homepage from cart page', async ({ page }) => {
    await page.getByTestId('cart-link').click();
    await page.getByTestId('continue-shopping-button').click();
    
    await expect(page).toHaveURL('/');
    await expect(page.getByTestId('home-page')).toBeVisible();
  });
});
