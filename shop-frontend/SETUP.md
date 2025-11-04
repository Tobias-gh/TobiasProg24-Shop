# Quick Setup Guide

## ✅ What's Been Created

Your frontend is now fully set up with:

1. ✅ Tailwind CSS + shadcn/ui styling
2. ✅ TypeScript types matching your API
3. ✅ API client layer (axios)
4. ✅ Cart state management (React Context)
5. ✅ All core components (ProductCard, Cart, etc.)
6. ✅ Three pages: Home, Product Detail, Cart
7. ✅ React Router navigation

## 🚀 Next Steps

### 1. Configure Your API URL

**Important:** Open `src/api/apiClient.ts` and update the API URL:

```typescript
const API_BASE_URL = 'http://localhost:5000';  // ← Change this to match your backend
```

Common ports for ASP.NET Core:
- Development: `http://localhost:5000` or `http://localhost:5001` (HTTPS)
- Check your `launchSettings.json` for the exact port

### 2. Enable CORS on Your Backend

Add this to your `Program.cs` if not already there:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // Frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// After var app = builder.Build();
app.UseCors();
```

### 3. Start Everything

**Terminal 1 - Backend:**
```bash
cd c:\Users\Toby\source\repos\shop\src\server\Shop.Api
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd c:\Users\Toby\Desktop\ShopFrontend\shop-frontend
npm run dev
```

### 4. Test the Application

1. Open http://localhost:5173
2. You should see products listed (if your API has data)
3. Try:
   - Filtering by category
   - Clicking on a product for details
   - Adding items to cart
   - Viewing cart
   - Updating quantities

## 🔍 Current Status

- **Frontend running:** ✅ http://localhost:5173
- **Backend needed:** Make sure your API is running on the correct port

## 📝 API Endpoints Used

Your frontend will call:
- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get single product
- `GET /api/categories` - List categories
- `GET /api/carts/{sessionId}` - Get cart
- `POST /api/carts/{sessionId}/items` - Add to cart
- `PUT /api/carts/{sessionId}/items/{itemId}` - Update quantity
- `DELETE /api/carts/{sessionId}/items/{itemId}` - Remove item

## 🎨 Features Included

### Home Page (/)
- Product grid with cards
- Category filtering
- Add to cart from cards
- "View Details" button

### Product Detail Page (/product/:id)
- Full product information
- Quantity selector
- Add to cart with custom quantity
- Stock availability

### Cart Page (/cart)
- List all cart items
- Update quantities (+/-)
- Remove items
- View total price
- Clear cart

### Header (on all pages)
- Navigation links
- Cart icon with item count badge

## 🐛 Common Issues

### "Network Error" or API not connecting
- Check if backend is running
- Verify the port in `src/api/apiClient.ts`
- Check browser console for CORS errors

### Products not showing
- Make sure your database has products
- Check browser DevTools Network tab for API responses

### Cart not persisting
- Cart uses sessionId stored in localStorage
- Clear localStorage in DevTools if you need to reset

## 💡 What to Do Next?

**Option A - Test with your backend:**
1. Update the API URL
2. Make sure backend is running
3. Test all features

**Option B - Add features (let me know if you want any of these):**
- Search functionality
- Pagination
- Product images
- User authentication
- Order history
- Checkout flow
- Payment integration

Let me know what you'd like to do next! 🚀
