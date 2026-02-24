using Microsoft.EntityFrameworkCore;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Data;

namespace NVBM.Infrastructure.Services;

public class ProductCatalogService : IProductCatalogService
{
    private readonly IProductRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public ProductCatalogService(IProductRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<PagedResponse<ProductListDto>> GetProductsAsync(int page, int pageSize, Guid? categoryId, string? searchTerm)
    {
        var (totalCount, items) = await _repository.GetProductsAsync(page, pageSize, categoryId, searchTerm);

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
        return await _repository.GetProductDetailByIdAsync(id);
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

        _repository.Add(product);
        await _repository.SaveChangesAsync();
        
        await _eventPublisher.PublishProductChangedEventAsync(product.Id, "Created");
        
        return product.Id;
    }

    public async Task<bool> UpdateProductAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _repository.GetProductByIdAsync(id, includeDetails: true);

        if (product == null) return false;

        product.Sku = dto.Sku;
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;

        // EAV and UOM list replacement strategy
        _repository.RemoveUoms(product.Uoms);
        product.Uoms = dto.Uoms.Select(u => new ProductUom
        {
            Id = Guid.NewGuid(),
            UomCode = u.UomCode,
            UomName = u.UomName,
            Price = u.Price,
            ConversionFactor = u.ConversionFactor
        }).ToList();

        _repository.RemoveAttributes(product.Attributes);
        product.Attributes = dto.Attributes.Select(a => new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Key = a.Key,
            Value = a.Value
        }).ToList();

        await _repository.SaveChangesAsync();
        
        await _eventPublisher.PublishProductChangedEventAsync(product.Id, "Updated");
        
        return true;
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await _repository.GetProductByIdAsync(id);

        if (product == null) return false;

        // Soft Delete
        product.IsActive = false;
        
        await _repository.SaveChangesAsync();
        
        await _eventPublisher.PublishProductChangedEventAsync(product.Id, "Deleted");
        
        return true;
    }
}
