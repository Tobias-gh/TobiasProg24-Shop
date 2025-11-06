import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import '@testing-library/jest-dom';
import { Button } from '../../../src/components/ui/button'; 

describe('Button', () => {
  describe('rendering', () => {
    it('should render with children text', () => {
      // Arrange & Act
      render(<Button>Click me</Button>);

      // Assert
      expect(screen.getByRole('button', { name: /click me/i })).toBeInTheDocument();
    });

    it('should render as disabled when disabled prop is true', () => {
      // Arrange & Act
      render(<Button disabled>Disabled Button</Button>);

      // Assert
      expect(screen.getByRole('button')).toBeDisabled();
    });

    it('should apply default variant styles', () => {
      // Arrange & Act
      render(<Button>Default</Button>);

      // Assert
      const button = screen.getByRole('button');
      expect(button).toHaveClass('bg-primary');
    });

    it('should apply destructive variant styles', () => {
      // Arrange & Act
      render(<Button variant="destructive">Delete</Button>);

      // Assert
      const button = screen.getByRole('button');
      expect(button).toHaveClass('bg-destructive');
    });

    it('should apply outline variant styles', () => {
      // Arrange & Act
      render(<Button variant="outline">Outline</Button>);

      // Assert
      const button = screen.getByRole('button');
      expect(button).toHaveClass('border');
    });

    it('should apply secondary variant styles', () => {
      // Arrange & Act
      render(<Button variant="secondary">Secondary</Button>);

      // Assert
      const button = screen.getByRole('button');
      expect(button).toHaveClass('bg-secondary');
    });

    it('should apply ghost variant styles', () => {
      // Arrange & Act
      render(<Button variant="ghost">Ghost</Button>);

      // Assert
      const button = screen.getByRole('button');
      expect(button).toHaveClass('hover:bg-accent');
    });

    it('should apply link variant styles', () => {
      // Arrange & Act
      render(<Button variant="link">Link</Button>);

      // Assert
      const button = screen.getByRole('button');
      expect(button).toHaveClass('underline-offset-4');
    });

    it('should apply small size styles', () => {
      // Arrange & Act
      render(<Button size="sm">Small</Button>);

      // Assert
      const button = screen.getByRole('button');
      expect(button).toHaveClass('h-9');
    });

    it('should apply large size styles', () => {
      // Arrange & Act
      render(<Button size="lg">Large</Button>);

      // Assert
      const button = screen.getByRole('button');
      expect(button).toHaveClass('h-11');
    });

    it('should apply icon size styles', () => {
      // Arrange & Act
      render(<Button size="icon">🔍</Button>);

      // Assert
      const button = screen.getByRole('button');
      expect(button).toHaveClass('h-10');
      expect(button).toHaveClass('w-10');
    });

    it('should apply custom className', () => {
      // Arrange & Act
      render(<Button className="custom-class">Custom</Button>);

      // Assert
      expect(screen.getByRole('button')).toHaveClass('custom-class');
    });
  });

  describe('interactions', () => {
    it('should call onClick handler when clicked', async () => {
      // Arrange
      const handleClick = vi.fn();
      const user = userEvent.setup();
      render(<Button onClick={handleClick}>Click me</Button>);

      // Act
      await user.click(screen.getByRole('button'));

      // Assert
      expect(handleClick).toHaveBeenCalledTimes(1);
    });

    it('should not call onClick when disabled', async () => {
      // Arrange
      const handleClick = vi.fn();
      const user = userEvent.setup();
      render(<Button onClick={handleClick} disabled>Disabled</Button>);

      // Act
      await user.click(screen.getByRole('button'));

      // Assert
      expect(handleClick).not.toHaveBeenCalled();
    });

    it('should support keyboard navigation', async () => {
      // Arrange
      const handleClick = vi.fn();
      const user = userEvent.setup();
      render(<Button onClick={handleClick}>Press Enter</Button>);

      // Act
      const button = screen.getByRole('button');
      button.focus();
      await user.keyboard('{Enter}');

      // Assert
      expect(handleClick).toHaveBeenCalledTimes(1);
    });

    it('should support space key activation', async () => {
      // Arrange
      const handleClick = vi.fn();
      const user = userEvent.setup();
      render(<Button onClick={handleClick}>Press Space</Button>);

      // Act
      const button = screen.getByRole('button');
      button.focus();
      await user.keyboard(' ');

      // Assert
      expect(handleClick).toHaveBeenCalledTimes(1);
    });
  });

  describe('edge cases', () => {
    it('should render without onClick handler', () => {
      // Arrange & Act
      render(<Button>No handler</Button>);

      // Assert
      expect(screen.getByRole('button')).toBeInTheDocument();
    });

    it('should render with empty children', () => {
      // Arrange & Act
      render(<Button></Button>);

      // Assert
      expect(screen.getByRole('button')).toBeInTheDocument();
    });

    it('should handle type prop', () => {
      // Arrange & Act
      render(<Button type="submit">Submit</Button>);

      // Assert
      expect(screen.getByRole('button')).toHaveAttribute('type', 'submit');
    });

    it('should render as child component (asChild prop)', () => {
      // Arrange & Act
      render(
        <Button asChild>
          <a href="/test">Link Button</a>
        </Button>
      );

      // Assert
      expect(screen.getByRole('link')).toHaveAttribute('href', '/test');
    });
  });
});