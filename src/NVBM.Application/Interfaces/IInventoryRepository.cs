using NVBM.Domain.Entities;

namespace NVBM.Application.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryLevel?> GetInventoryLevelAsync(Guid productId, CancellationToken cancellationToken);
    Task<ProductUom?> GetProductUomAsync(Guid productId, Guid uomId, CancellationToken cancellationToken);
    void UpdateInventoryRowVersion(InventoryLevel inventory, byte[] originalRowVersion);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
