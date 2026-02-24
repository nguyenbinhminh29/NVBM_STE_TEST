using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;

namespace NVBM.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductCatalogService _productService;

    public ProductsController(IProductCatalogService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    // [OutputCache(Duration = 300)]
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

    [HttpPost]
    public async Task<ActionResult<APIResponse<Guid>>> CreateProduct([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(APIResponse<Guid>.Fail("Validation failed"));
        }

        var id = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id }, APIResponse<Guid>.Ok(id));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<APIResponse<bool>>> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
    {
        var success = await _productService.UpdateProductAsync(id, dto);
        if (!success) return NotFound(APIResponse<bool>.Fail("Product not found or inactive"));
        
        return Ok(APIResponse<bool>.Ok(true));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<APIResponse<bool>>> DeleteProduct(Guid id)
    {
        var success = await _productService.DeleteProductAsync(id);
        if (!success) return NotFound(APIResponse<bool>.Fail("Product not found or already deleted"));
        
        return Ok(APIResponse<bool>.Ok(true));
    }
}
