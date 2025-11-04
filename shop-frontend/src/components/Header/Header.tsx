import { Link } from 'react-router-dom';
import { useCart } from '../../context/CartContext';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { ShoppingCart, Store } from 'lucide-react';

export const Header: React.FC = () => {
  const { cart } = useCart();

  return (
    <header data-testid="header" className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-backdrop-filter:bg-background/60">
      <div className="container mx-auto flex h-16 items-center justify-between px-4">
        <Link to="/" data-testid="logo-link" className="flex items-center gap-2 hover:opacity-80 transition-opacity">
          <Store className="w-6 h-6 text-primary" />
          <span className="text-xl font-bold">Shop</span>
        </Link>
        
        <nav className="flex items-center gap-4">
          <Link to="/">
            <Button data-testid="home-link" variant="ghost">Home</Button>
          </Link>
          <Link to="/cart">
            <Button data-testid="cart-link" variant="outline" className="relative">
              <ShoppingCart className="w-4 h-4 mr-2" />
              Cart
              {cart && cart.totalItems > 0 && (
                <Badge 
                  data-testid="cart-badge"
                  variant="destructive" 
                  className="absolute -top-2 -right-2 h-6 w-6 rounded-full p-0 flex items-center justify-center"
                >
                  {cart.totalItems}
                </Badge>
              )}
            </Button>
          </Link>
        </nav>
      </div>
    </header>
  );
};
