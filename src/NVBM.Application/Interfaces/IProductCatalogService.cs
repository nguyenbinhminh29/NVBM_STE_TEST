using NVBM.Application.DTOs;

namespace NVBM.Application.Interfaces;

public interface IProductCatalogService
{
    Task<PagedResponse<ProductListDto>> GetProductsAsync(int page, int pageSize, Guid? categoryId, string? searchTerm);
    Task<ProductDetailDto?> GetProductByIdAsync(Guid id);
}
