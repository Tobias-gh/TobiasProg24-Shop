import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./tests/setup.ts'],
    css: true,
    
    // ✅ Properly exclude e2e tests
    exclude: [
      '**/node_modules/**',
      '**/dist/**',
      '**/e2e/**',
      '**/.{idea,git,cache,output,temp}/**',
    ],
    
    // ✅ Only include test files in tests/ folder
    include: ['tests/**/*.{test,spec}.{ts,tsx}'],
    
    clearMocks: true,
    mockReset: true,
    restoreMocks: true,
    
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      reportsDirectory: './coverage',
      exclude: [
        'node_modules/',
        'tests/',
        'e2e/',
        '**/*.test.{ts,tsx}',
        '**/*.spec.{ts,tsx}',
        '**/index.ts',
        '**/*.config.{ts,js}',
        'src/main.tsx',
        'src/vite-env.d.ts',
        'src/assets/',
        'src/App.tsx',                   
        'src/components/**',              
        'src/context/**',                 
        'src/pages/**',  
      ],
      include: [
        'src/**/*.{ts,tsx}',
        'src/lib/**/*.{ts,tsx}', 
        'src/components/ui/**/*.{ts,tsx}',
      ],
     
      thresholds: {
        lines: 80,
        functions: 80,
        branches: 80,
        statements: 80,
      },
    },
  },
  
  resolve: {
    alias: {
      '@': path.resolve(__dirname, '../src'),
    },
  },
});