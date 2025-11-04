import { useNavigate } from 'react-router-dom';
import { Cart } from '../../components/Cart';
import { Button } from '../../components/ui/button';
import { ArrowLeft } from 'lucide-react';

export const CartPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <div data-testid="cart-page" className="container mx-auto px-4 py-8">
      <Button
        data-testid="continue-shopping-button"
        variant="ghost"
        onClick={() => navigate('/')}
        className="mb-6"
      >
        <ArrowLeft className="w-4 h-4 mr-2" />
        Continue Shopping
      </Button>

      <h1 data-testid="page-title" className="text-3xl font-bold mb-8">Your Shopping Cart</h1>

      <div className="max-w-4xl mx-auto">
        <Cart />
      </div>
    </div>
  );
};
