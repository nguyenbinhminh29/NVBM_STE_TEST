using Microsoft.EntityFrameworkCore;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Data;

namespace NVBM.Infrastructure.Services;

public class ProductCatalogService : IProductCatalogService
{
    private readonly NVBMDbContext _dbContext;
    private readonly IEventPublisher _eventPublisher;

    public ProductCatalogService(NVBMDbContext dbContext, IEventPublisher eventPublisher)
    {
        _dbContext = dbContext;
        _eventPublisher = eventPublisher;
    }

    public async Task<PagedResponse<ProductListDto>> GetProductsAsync(int page, int pageSize, Guid? categoryId, string? searchTerm)
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
            .FirstOrDefaultAsync();
    }

    public async Task<Guid> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Sku = dto.Sku,
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            IsActive = true,
            Uoms = dto.Uoms.Select(u => new ProductUom
            {
                Id = Guid.NewGuid(),
                UomCode = u.UomCode,
                UomName = u.UomName,
                Price = u.Price,
                ConversionFactor = u.ConversionFactor
            }).ToList(),
            Attributes = dto.Attributes.Select(a => new ProductAttribute
            {
                Id = Guid.NewGuid(),
                Key = a.Key,
                Value = a.Value
            }).ToList()
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        
        await _eventPublisher.PublishProductChangedEventAsync(product.Id, "Created");
        
        return product.Id;
    }

    public async Task<bool> UpdateProductAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _dbContext.Products
            .Include(p => p.Uoms)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product == null) return false;

        product.Sku = dto.Sku;
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;

        // EAV and UOM list replacement strategy
        _dbContext.ProductUoms.RemoveRange(product.Uoms);
        product.Uoms = dto.Uoms.Select(u => new ProductUom
        {
            Id = Guid.NewGuid(),
            UomCode = u.UomCode,
            UomName = u.UomName,
            Price = u.Price,
            ConversionFactor = u.ConversionFactor
        }).ToList();

        _dbContext.ProductAttributes.RemoveRange(product.Attributes);
        product.Attributes = dto.Attributes.Select(a => new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Key = a.Key,
            Value = a.Value
        }).ToList();

        await _dbContext.SaveChangesAsync();
        
        await _eventPublisher.PublishProductChangedEventAsync(product.Id, "Updated");
        
        return true;
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product == null) return false;

        // Soft Delete
        product.IsActive = false;
        
        await _dbContext.SaveChangesAsync();
        
        await _eventPublisher.PublishProductChangedEventAsync(product.Id, "Deleted");
        
        return true;
    }
}
