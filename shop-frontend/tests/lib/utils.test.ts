import { describe, it, expect } from 'vitest';
import { cn } from '../../src/lib/utils';



describe('utils', () => {
  describe('cn', () => {
    it('should merge class names correctly', () => {
      const result = cn('btn', 'btn-primary');
      expect(result).toBe('btn btn-primary');
    });

    it('should handle conditional classes', () => {
      const result = cn('btn', { 'btn-disabled': true, 'btn-active': false });
      expect(result).toContain('btn-disabled');
      expect(result).not.toContain('btn-active');
    });

    it('should handle undefined and null values', () => {
      const result = cn('btn', undefined, null, 'btn-primary');
      expect(result).toBe('btn btn-primary');
    });

    it('should merge Tailwind classes and remove conflicts', () => {
      // twMerge should handle conflicting tailwind classes
      const result = cn('px-2', 'px-4');
      expect(result).toBe('px-4'); // Later class wins
    });

    it('should handle empty input', () => {
      const result = cn();
      expect(result).toBe('');
    });

    it('should handle array of classes', () => {
      const result = cn(['btn', 'btn-primary']);
      expect(result).toContain('btn');
      expect(result).toContain('btn-primary');
    });

    it('should combine multiple class types', () => {
      const result = cn('base-class', { conditional: true }, ['array-class']);
      expect(result).toContain('base-class');
      expect(result).toContain('conditional');
      expect(result).toContain('array-class');
    });

    it('should handle boolean false values', () => {
      const result = cn('btn', false && 'hidden', 'visible');
      expect(result).toContain('btn');
      expect(result).toContain('visible');
      expect(result).not.toContain('hidden');
    });
  });
});
