import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom/vitest'
import { Badge } from '../../../src/components/ui/badge';

describe('Badge', () => {
  describe('rendering', () => {
    it('should render with children text', () => {
      // Arrange & Act
      render(<Badge>New</Badge>);

      // Assert
      expect(screen.getByText('New')).toBeInTheDocument();
    });

    it('should apply default variant styles', () => {
      // Arrange & Act
      const { container } = render(<Badge>Default</Badge>);

      // Assert
      expect(container.firstChild).toHaveClass('bg-primary');
    });

    it('should apply secondary variant styles', () => {
      // Arrange & Act
      const { container } = render(<Badge variant="secondary">Secondary</Badge>);

      // Assert
      expect(container.firstChild).toHaveClass('bg-secondary');
    });

    it('should apply destructive variant styles', () => {
      // Arrange & Act
      const { container } = render(<Badge variant="destructive">Error</Badge>);

      // Assert
      expect(container.firstChild).toHaveClass('bg-destructive');
    });

    it('should apply outline variant styles', () => {
      // Arrange & Act
      const { container } = render(<Badge variant="outline">Outline</Badge>);

      // Assert
      expect(container.firstChild).toHaveClass('border');
    });

    it('should apply custom className', () => {
      // Arrange & Act
      const { container } = render(<Badge className="custom-badge">Custom</Badge>);

      // Assert
      expect(container.firstChild).toHaveClass('custom-badge');
    });

    it('should have inline-flex display', () => {
      // Arrange & Act
      const { container } = render(<Badge>Badge</Badge>);

      // Assert
      expect(container.firstChild).toHaveClass('inline-flex');
    });

    it('should render with number content', () => {
      // Arrange & Act
      render(<Badge>5</Badge>);

      // Assert
      expect(screen.getByText('5')).toBeInTheDocument();
    });

    it('should render empty badge', () => {
      // Arrange & Act
      const { container } = render(<Badge></Badge>);

      // Assert
      expect(container.firstChild).toBeInTheDocument();
    });
  });

  describe('use cases', () => {
    it('should display out of stock badge', () => {
      // Arrange & Act
      render(<Badge variant="destructive">Out of Stock</Badge>);

      // Assert
      expect(screen.getByText('Out of Stock')).toBeInTheDocument();
    });

    it('should display category badge', () => {
      // Arrange & Act
      render(<Badge variant="secondary">Electronics</Badge>);

      // Assert
      expect(screen.getByText('Electronics')).toBeInTheDocument();
    });

    it('should display cart count badge', () => {
      // Arrange & Act
      render(<Badge>3</Badge>);

      // Assert
      expect(screen.getByText('3')).toBeInTheDocument();
    });
  });
});