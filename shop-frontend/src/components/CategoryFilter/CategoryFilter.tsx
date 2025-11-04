import { useEffect, useState } from 'react';
import type { Category } from '../../types';
import { categoriesApi } from '../../api';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Loader2 } from 'lucide-react';

interface CategoryFilterProps {
  selectedCategoryId?: string;
  onCategoryChange: (categoryId?: string) => void;
}

export const CategoryFilter: React.FC<CategoryFilterProps> = ({
  selectedCategoryId,
  onCategoryChange,
}) => {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const data = await categoriesApi.getAll();
        setCategories(data);
      } catch (error) {
        console.error('Error loading categories:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchCategories();
  }, []);

  if (loading) {
    return (
      <div className="flex gap-2">
        <Loader2 className="w-5 h-5 animate-spin" />
      </div>
    );
  }

  return (
    <div className="flex flex-wrap gap-2">
      <Button
        variant={!selectedCategoryId ? 'default' : 'outline'}
        onClick={() => onCategoryChange(undefined)}
        size="sm"
      >
        All Products
      </Button>
      {categories.map((category) => (
        <Button
          key={category.id}
          variant={selectedCategoryId === category.id ? 'default' : 'outline'}
          onClick={() => onCategoryChange(category.id)}
          size="sm"
        >
          {category.name}
          <Badge variant="secondary" className="ml-2">
            {category.productCount}
          </Badge>
        </Button>
      ))}
    </div>
  );
};
