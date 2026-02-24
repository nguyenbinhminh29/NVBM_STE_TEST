using Microsoft.EntityFrameworkCore;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Data;

namespace NVBM.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly NVBMDbContext _dbContext;

    public ProductRepository(NVBMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(int TotalCount, List<ProductListDto> Items)> GetProductsAsync(int page, int pageSize, Guid? categoryId, string? searchTerm, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Sku.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

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
            .ToListAsync(cancellationToken);

        return (totalCount, items);
    }

    public async Task<ProductDetailDto?> GetProductDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                BaseUom = p.Uoms.Any(u => u.ConversionFactor == 1m) ? p.Uoms.FirstOrDefault(u => u.ConversionFactor == 1m)!.UomCode : string.Empty,
                AvailableUnits = p.Uoms.Select(u => new ProductUomDto
                {
                    Id = u.Id,
                    UomCode = u.UomCode,
                    UomName = u.UomName,
                    Price = u.Price,
                    ConversionFactor = u.ConversionFactor,
                    IsDefault = u.ConversionFactor == 1m
                }).ToList(),
                Attributes = p.Attributes.Select(a => new ProductAttributeDto
                {
                    Key = a.Key,
                    Value = a.Value
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products.AsQueryable();

        if (includeDetails)
        {
            query = query.Include(p => p.Uoms).Include(p => p.Attributes);
        }

        return await query.FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public void Add(Product product)
    {
        _dbContext.Products.Add(product);
    }

    public void RemoveUoms(IEnumerable<ProductUom> uoms)
    {
        _dbContext.ProductUoms.RemoveRange(uoms);
    }

    public void RemoveAttributes(IEnumerable<ProductAttribute> attributes)
    {
        _dbContext.ProductAttributes.RemoveRange(attributes);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
