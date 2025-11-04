import { useEffect, useState } from 'react';
import type { Product } from '../../types';
import { productsApi } from '../../api';
import { ProductCard } from '../ProductCard';
import { Loader2 } from 'lucide-react';

interface ProductListProps {
  categoryId?: string;
  onViewDetails?: (productId: string) => void;
}

export const ProductList: React.FC<ProductListProps> = ({ categoryId, onViewDetails }) => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = categoryId
          ? await productsApi.getByCategory(categoryId)
          : await productsApi.getAll();
        setProducts(data);
      } catch (err) {
        setError('Failed to load products');
        console.error('Error loading products:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, [categoryId]);

  if (loading) {
    return (
      <div data-testid="product-list-loading" className="flex justify-center items-center py-12">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
      </div>
    );
  }

  if (error) {
    return (
      <div data-testid="product-list-error" className="text-center py-12">
        <p className="text-destructive">{error}</p>
      </div>
    );
  }

  if (products.length === 0) {
    return (
      <div data-testid="product-list-empty" className="text-center py-12">
        <p className="text-muted-foreground">No products found</p>
      </div>
    );
  }

  return (
    <div data-testid="product-list" className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      {products.map((product) => (
        <ProductCard
          key={product.id}
          product={product}
          onViewDetails={onViewDetails}
        />
      ))}
    </div>
  );
};
