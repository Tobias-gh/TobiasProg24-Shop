using Shop.Application.Dtos;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Repositories;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services.Category;

public class CategoryService : ICategoryService
{

    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }


    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryDto(
            c.Id,
            c.Name,
            c.Description,
            c.Products?.Count ?? 0
            ));
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return null;

        return new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.Products?.Count ?? 0
        );
    }
}

