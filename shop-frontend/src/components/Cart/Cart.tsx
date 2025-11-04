import { useCart } from '../../context/CartContext';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Minus, Plus, Trash2, ShoppingCart, Loader2 } from 'lucide-react';
import { useState } from 'react';

export const Cart: React.FC = () => {
  const { cart, loading, updateQuantity, removeFromCart, clearCart } = useCart();
  const [updatingItems, setUpdatingItems] = useState<Set<string>>(new Set());

  const handleUpdateQuantity = async (itemId: string, newQuantity: number) => {
    if (newQuantity < 1) return;
    
    setUpdatingItems(prev => new Set(prev).add(itemId));
    try {
      await updateQuantity(itemId, newQuantity);
    } finally {
      setUpdatingItems(prev => {
        const newSet = new Set(prev);
        newSet.delete(itemId);
        return newSet;
      });
    }
  };

  const handleRemoveItem = async (itemId: string) => {
    setUpdatingItems(prev => new Set(prev).add(itemId));
    try {
      await removeFromCart(itemId);
    } finally {
      setUpdatingItems(prev => {
        const newSet = new Set(prev);
        newSet.delete(itemId);
        return newSet;
      });
    }
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(price);
  };

  if (loading && !cart) {
    return (
      <Card data-testid="cart-loading">
        <CardContent className="flex justify-center items-center py-12">
          <Loader2 className="w-8 h-8 animate-spin" />
        </CardContent>
      </Card>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <Card data-testid="cart-empty">
        <CardContent className="flex flex-col items-center justify-center py-12 text-center">
          <ShoppingCart className="w-16 h-16 text-muted-foreground mb-4" />
          <p data-testid="empty-cart-message" className="text-lg font-semibold mb-2">Your cart is empty</p>
          <p className="text-muted-foreground">Add some products to get started!</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card data-testid="cart">
      <CardHeader>
        <CardTitle className="flex justify-between items-center">
          <span>Shopping Cart</span>
          <Badge data-testid="cart-total-items" variant="secondary">
            {cart.totalItems} {cart.totalItems === 1 ? 'item' : 'items'}
          </Badge>
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {cart.items.map((item) => {
          const isUpdating = updatingItems.has(item.id);
          return (
            <div
              key={item.id}
              data-testid="cart-item"
              className="flex gap-4 p-4 border rounded-lg hover:bg-accent/50 transition-colors"
            >
              <div className="flex-1">
                <h3 data-testid="cart-item-name" className="font-semibold">{item.productName}</h3>
                <p data-testid="cart-item-description" className="text-sm text-muted-foreground line-clamp-1">
                  {item.productDescription}
                </p>
                <div className="flex items-center gap-4 mt-2">
                  <span data-testid="cart-item-price" className="font-semibold text-primary">
                    {formatPrice(item.productPrice)}
                  </span>
                  <Badge data-testid="cart-item-stock" variant="outline" className="text-xs">
                    Stock: {item.availableStock}
                  </Badge>
                </div>
              </div>
              <div className="flex flex-col items-end justify-between">
                <div className="flex items-center gap-2">
                  <Button
                    data-testid="decrease-quantity"
                    size="icon"
                    variant="outline"
                    className="h-8 w-8"
                    onClick={() => handleUpdateQuantity(item.id, item.quantity - 1)}
                    disabled={isUpdating || item.quantity <= 1}
                  >
                    <Minus className="w-4 h-4" />
                  </Button>
                  <span data-testid="item-quantity" className="w-12 text-center font-semibold">
                    {item.quantity}
                  </span>
                  <Button
                    data-testid="increase-quantity"
                    size="icon"
                    variant="outline"
                    className="h-8 w-8"
                    onClick={() => handleUpdateQuantity(item.id, item.quantity + 1)}
                    disabled={isUpdating || item.quantity >= item.availableStock}
                  >
                    <Plus className="w-4 h-4" />
                  </Button>
                  <Button
                    data-testid="remove-item"
                    size="icon"
                    variant="destructive"
                    className="h-8 w-8 ml-2"
                    onClick={() => handleRemoveItem(item.id)}
                    disabled={isUpdating}
                  >
                    <Trash2 className="w-4 h-4" />
                  </Button>
                </div>
                <div className="text-right mt-2">
                  <p className="text-sm text-muted-foreground">Subtotal:</p>
                  <p data-testid="cart-item-subtotal" className="text-lg font-bold text-primary">
                    {formatPrice(item.subtotal)}
                  </p>
                </div>
              </div>
            </div>
          );
        })}
      </CardContent>
      <CardFooter className="flex flex-col gap-4">
        <div className="w-full flex justify-between items-center text-xl font-bold border-t pt-4">
          <span>Total:</span>
          <span data-testid="cart-total-price" className="text-primary">{formatPrice(cart.totalPrice)}</span>
        </div>
        <div className="w-full flex gap-2">
          <Button
            data-testid="clear-cart-button"
            variant="outline"
            className="flex-1"
            onClick={clearCart}
            disabled={loading}
          >
            Clear Cart
          </Button>
          <Button data-testid="checkout-button" className="flex-1" disabled={loading}>
            Checkout
          </Button>
        </div>
      </CardFooter>
    </Card>
  );
};
