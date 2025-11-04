using Shop.Application.Dtos;
using Shop.Application.Services.Category;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services.Product;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    //private readonly ICategoryService _categoryService;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        //_categoryService = categoryService;
    }


    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        return product != null ? MapToDto(product) : null;
    }

    private static ProductDto MapToDto(Domain.Entities.Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.Category?.Name ?? "Uncategorized"
        );
    }
}

