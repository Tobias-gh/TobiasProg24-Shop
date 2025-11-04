import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ProductList } from '../../components/ProductList';
import { CategoryFilter } from '../../components/CategoryFilter';

export const HomePage: React.FC = () => {
  const [selectedCategory, setSelectedCategory] = useState<string | undefined>();
  const navigate = useNavigate();

  const handleViewDetails = (productId: string) => {
    navigate(`/product/${productId}`);
  };

  return (
    <div data-testid="home-page" className="container mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 data-testid="page-title" className="text-4xl font-bold mb-2">Welcome to Our Shop</h1>
        <p data-testid="page-subtitle" className="text-muted-foreground">
          Browse our collection of quality products
        </p>
      </div>

      <div className="mb-6">
        <h2 className="text-lg font-semibold mb-3">Filter by Category</h2>
        <CategoryFilter
          selectedCategoryId={selectedCategory}
          onCategoryChange={setSelectedCategory}
        />
      </div>

      <ProductList
        categoryId={selectedCategory}
        onViewDetails={handleViewDetails}
      />
    </div>
  );
};
