
import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom/vitest';
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from '../../../src/components/ui/card'; 


describe('Card Components', () => {
  describe('Card', () => {
    it('should render children', () => {
      // Arrange & Act
      render(<Card>Card content</Card>);

      // Assert
      expect(screen.getByText('Card content')).toBeInTheDocument();
    });

    it('should apply custom className', () => {
      // Arrange & Act
      const { container } = render(<Card className="custom-card">Content</Card>);

      // Assert
      expect(container.firstChild).toHaveClass('custom-card');
    });

    it('should have default card styles', () => {
      // Arrange & Act
      const { container } = render(<Card>Content</Card>);

      // Assert
      expect(container.firstChild).toHaveClass('rounded-lg');
      expect(container.firstChild).toHaveClass('border');
    });
  });

  describe('CardHeader', () => {
    it('should render children', () => {
      // Arrange & Act
      render(
        <Card>
          <CardHeader data-testid="card-header">Header content</CardHeader>
        </Card>
      );

      // Assert
      expect(screen.getByText('Header content')).toBeInTheDocument();
    });

    it('should apply custom className', () => {
      // Arrange & Act
      render(
        <Card>
          <CardHeader className="custom-header" data-testid="card-header">Header</CardHeader>
        </Card>
      );

      // Assert
      const header = screen.getByTestId('card-header');
      expect(header).toHaveClass('custom-header');
    });
  });

  describe('CardTitle', () => {
    it('should render as h3 by default', () => {
      // Arrange & Act
      render(
        <Card>
          <CardHeader>
            <CardTitle>Card Title</CardTitle>
          </CardHeader>
        </Card>
      );

      // Assert
      const title = screen.getByText('Card Title');
      expect(title.tagName).toBe('H3');
    });

    it('should apply title styles', () => {
      // Arrange & Act
      render(
        <Card>
          <CardHeader>
            <CardTitle>Title</CardTitle>
          </CardHeader>
        </Card>
      );

      // Assert
      const title = screen.getByText('Title');
      expect(title).toHaveClass('font-semibold');
    });
  });

  describe('CardDescription', () => {
    it('should render description text', () => {
      // Arrange & Act
      render(
        <Card>
          <CardHeader>
            <CardDescription>Card description text</CardDescription>
          </CardHeader>
        </Card>
      );

      // Assert
      expect(screen.getByText('Card description text')).toBeInTheDocument();
    });

    it('should have muted text style', () => {
      // Arrange & Act
      render(
        <Card>
          <CardHeader>
            <CardDescription>Description</CardDescription>
          </CardHeader>
        </Card>
      );

      // Assert
      expect(screen.getByText('Description')).toHaveClass('text-muted-foreground');
    });
  });

  describe('CardContent', () => {
    it('should render content', () => {
      // Arrange & Act
      render(
        <Card>
          <CardContent>Card body content</CardContent>
        </Card>
      );

      // Assert
      expect(screen.getByText('Card body content')).toBeInTheDocument();
    });

    it('should apply padding', () => {
      // Arrange & Act
      render(
        <Card>
          <CardContent data-testid="card-content">Content</CardContent>
        </Card>
      );

      // Assert
      const content = screen.getByTestId('card-content');
      expect(content).toHaveClass('p-6');
    });
  });

  describe('CardFooter', () => {
    it('should render footer content', () => {
      // Arrange & Act
      render(
        <Card>
          <CardFooter>Footer content</CardFooter>
        </Card>
      );

      // Assert
      expect(screen.getByText('Footer content')).toBeInTheDocument();
    });

    it('should apply flex layout', () => {
      // Arrange & Act
      render(
        <Card>
          <CardFooter data-testid="card-footer">Footer</CardFooter>
        </Card>
      );

      // Assert
      const footer = screen.getByTestId('card-footer');
      expect(footer).toHaveClass('flex');
    });
  });

  describe('Complete Card', () => {
    it('should render complete card structure', () => {
      // Arrange & Act
      render(
        <Card>
          <CardHeader>
            <CardTitle>Product Name</CardTitle>
            <CardDescription>Product description</CardDescription>
          </CardHeader>
          <CardContent>
            <p>Main content</p>
          </CardContent>
          <CardFooter>
            <button>Action</button>
          </CardFooter>
        </Card>
      );

      // Assert
      expect(screen.getByText('Product Name')).toBeInTheDocument();
      expect(screen.getByText('Product description')).toBeInTheDocument();
      expect(screen.getByText('Main content')).toBeInTheDocument();
      expect(screen.getByRole('button', { name: /action/i })).toBeInTheDocument();
    });
  });
});