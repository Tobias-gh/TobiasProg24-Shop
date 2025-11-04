import type { Product } from '../../types';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { ShoppingCart, Package } from 'lucide-react';
import { useCart } from '../../context/CartContext';
import { useState } from 'react';

interface ProductCardProps {
  product: Product;
  onViewDetails?: (productId: string) => void;
}

export const ProductCard: React.FC<ProductCardProps> = ({ product, onViewDetails }) => {
  const { addToCart } = useCart();
  const [isAdding, setIsAdding] = useState(false);

  const handleAddToCart = async () => {
    try {
      setIsAdding(true);
      await addToCart({ productId: product.id, quantity: 1 });
    } catch (error) {
      console.error('Failed to add to cart:', error);
    } finally {
      setIsAdding(false);
    }
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(price);
  };

  return (
    <Card 
      className="h-full flex flex-col hover:shadow-lg transition-shadow"
      data-testid="product-card"
    >
      <CardHeader>
        <div className="flex justify-between items-start">
          <CardTitle 
            className="text-lg"
            data-testid="product-title"
          >
            {product.name}
          </CardTitle>
          <Badge 
            variant={product.stock > 0 ? 'default' : 'destructive'}
            data-testid="product-stock-badge"
          >
            <Package className="w-3 h-3 mr-1" />
            {product.stock}
          </Badge>
        </div>
        <Badge variant="outline" className="w-fit">
          {product.categoryName}
        </Badge>
      </CardHeader>
      <CardContent className="flex-1">
        <CardDescription 
          className="line-clamp-3"
          data-testid="product-description"
        >
          {product.description}
        </CardDescription>
      </CardContent>
      <CardFooter className="flex justify-between items-center">
        <div 
          className="text-2xl font-bold text-primary"
          data-testid="product-price"
        >
          {formatPrice(product.price)}
        </div>
        <div className="flex gap-2">
          {onViewDetails && (
            <Button
              variant="outline"
              size="sm"
              onClick={() => onViewDetails(product.id)}
              data-testid="view-details-button"
            >
              Details
            </Button>
          )}
          <Button
            size="sm"
            onClick={handleAddToCart}
            disabled={product.stock === 0 || isAdding}
            data-testid="add-to-cart-button"
          >
            <ShoppingCart className="w-4 h-4 mr-1" />
            {isAdding ? 'Adding...' : 'Add'}
          </Button>
        </div>
      </CardFooter>
    </Card>
  );
};
