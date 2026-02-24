using NVBM.Application.DTOs;
using NVBM.Domain.Entities;

namespace NVBM.Application.Interfaces;

public interface IProductRepository
{
    Task<(int TotalCount, List<ProductListDto> Items)> GetProductsAsync(int page, int pageSize, Guid? categoryId, string? searchTerm, CancellationToken cancellationToken = default);
    Task<ProductDetailDto?> GetProductDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);
    
    void Add(Product product);
    void RemoveUoms(IEnumerable<ProductUom> uoms);
    void RemoveAttributes(IEnumerable<ProductAttribute> attributes);
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
