using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;

namespace NVBM.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[OutputCache(Duration = 300)]
public class ProductsController : ControllerBase
{
    private readonly IProductCatalogService _productService;

    public ProductsController(IProductCatalogService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [EnableRateLimiting("fixed")]
    public async Task<ActionResult<APIResponse<PagedResponse<ProductListDto>>>> GetProducts(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] Guid? categoryId = null, 
        [FromQuery] string? searchTerm = null)
    {
        var result = await _productService.GetProductsAsync(page, pageSize, categoryId, searchTerm);
        return Ok(APIResponse<PagedResponse<ProductListDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<APIResponse<ProductDetailDto>>> GetProduct(Guid id)
    {
        var result = await _productService.GetProductByIdAsync(id);

        if (result == null)
        {
            return NotFound(APIResponse<ProductDetailDto>.Fail("Product not found"));
        }

        return Ok(APIResponse<ProductDetailDto>.Ok(result));
    }
}
