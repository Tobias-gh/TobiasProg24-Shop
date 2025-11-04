using Microsoft.AspNetCore.Mvc;
using Shop.Application.Dtos;
using Shop.Application.Services.Product;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetbyId(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new {message = $"product with ID {id} not found"});
        }
        return Ok(product);
    }
}
