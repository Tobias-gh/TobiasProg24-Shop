import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import type { Product } from '../../types';
import { productsApi } from '../../api';
import { useCart } from '../../context/CartContext';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Badge } from '../../components/ui/badge';
import { ArrowLeft, ShoppingCart, Package, Loader2, Minus, Plus } from 'lucide-react';

export const ProductDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { addToCart } = useCart();
  
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [isAdding, setIsAdding] = useState(false);

  useEffect(() => {
    const fetchProduct = async () => {
      if (!id) return;
      
      try {
        setLoading(true);
        const data = await productsApi.getById(id);
        setProduct(data);
      } catch (err) {
        setError('Failed to load product details');
        console.error('Error loading product:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [id]);

  const handleAddToCart = async () => {
    if (!product) return;
    
    try {
      setIsAdding(true);
      await addToCart({ productId: product.id, quantity });
      // Show success feedback
      alert('Added to cart successfully!');
    } catch (error) {
      console.error('Failed to add to cart:', error);
      alert('Failed to add to cart');
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

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="flex justify-center items-center min-h-[400px]">
          <Loader2 className="w-8 h-8 animate-spin text-primary" />
        </div>
      </div>
    );
  }

  if (error || !product) {
    return (
      <div className="container mx-auto px-4 py-8">
        <Card>
          <CardContent className="py-12 text-center">
            <p className="text-destructive mb-4">{error || 'Product not found'}</p>
            <Button onClick={() => navigate('/')}>
              <ArrowLeft className="w-4 h-4 mr-2" />
              Back to Shop
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <Button
        variant="ghost"
        onClick={() => navigate('/')}
        className="mb-6"
      >
        <ArrowLeft className="w-4 h-4 mr-2" />
        Back to Products
      </Button>

      <div className="grid md:grid-cols-2 gap-8">
        <Card>
          <CardContent className="flex items-center justify-center min-h-[400px] bg-muted/20">
            <Package className="w-32 h-32 text-muted-foreground" />
          </CardContent>
        </Card>

        <div className="space-y-6">
          <div>
            <div className="flex items-start justify-between mb-2">
              <h1 className="text-3xl font-bold">{product.name}</h1>
              <Badge variant={product.stock > 0 ? 'default' : 'destructive'}>
                {product.stock > 0 ? `${product.stock} in stock` : 'Out of stock'}
              </Badge>
            </div>
            <Badge variant="outline" className="mb-4">
              {product.categoryName}
            </Badge>
          </div>

          <div className="text-4xl font-bold text-primary">
            {formatPrice(product.price)}
          </div>

          <Card>
            <CardHeader>
              <CardTitle>Description</CardTitle>
            </CardHeader>
            <CardContent>
              <CardDescription className="text-base">
                {product.description}
              </CardDescription>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Add to Cart</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex items-center gap-4">
                <span className="font-semibold">Quantity:</span>
                <div className="flex items-center gap-2">
                  <Button
                    size="icon"
                    variant="outline"
                    onClick={() => setQuantity(Math.max(1, quantity - 1))}
                    disabled={quantity <= 1}
                  >
                    <Minus className="w-4 h-4" />
                  </Button>
                  <span className="w-16 text-center font-semibold text-lg">
                    {quantity}
                  </span>
                  <Button
                    size="icon"
                    variant="outline"
                    onClick={() => setQuantity(Math.min(product.stock, quantity + 1))}
                    disabled={quantity >= product.stock}
                  >
                    <Plus className="w-4 h-4" />
                  </Button>
                </div>
              </div>
            </CardContent>
            <CardFooter>
              <Button
                className="w-full"
                size="lg"
                onClick={handleAddToCart}
                disabled={product.stock === 0 || isAdding}
              >
                <ShoppingCart className="w-5 h-5 mr-2" />
                {isAdding ? 'Adding...' : `Add to Cart - ${formatPrice(product.price * quantity)}`}
              </Button>
            </CardFooter>
          </Card>
        </div>
      </div>
    </div>
  );
};
