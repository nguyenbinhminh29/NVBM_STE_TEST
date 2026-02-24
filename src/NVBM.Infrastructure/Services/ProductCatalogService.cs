using Microsoft.EntityFrameworkCore;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;
using NVBM.Infrastructure.Data;

namespace NVBM.Infrastructure.Services;

public class ProductCatalogService : IProductCatalogService
{
    private readonly NVBMDbContext _dbContext;

    public ProductCatalogService(NVBMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<ProductListDto>> GetProductsAsync(int page, int pageSize, Guid? categoryId, string? searchTerm)
    {
        var query = _dbContext.Products
            .AsNoTracking()
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Sku.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                CategoryName = p.Category!.Name,
                MinPrice = p.Uoms.Any() ? p.Uoms.Min(u => u.Price) : 0
            })
            .ToListAsync();

        return new PagedResponse<ProductListDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProductDetailDto?> GetProductByIdAsync(Guid id)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                Uoms = p.Uoms.Select(u => new ProductUomDto
                {
                    Id = u.Id,
                    UomCode = u.UomCode,
                    UomName = u.UomName,
                    Price = u.Price
                }).ToList(),
                Attributes = p.Attributes.Select(a => new ProductAttributeDto
                {
                    Key = a.Key,
                    Value = a.Value
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}
