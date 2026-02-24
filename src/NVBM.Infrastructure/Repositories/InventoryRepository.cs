using Microsoft.EntityFrameworkCore;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Data;

namespace NVBM.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly NVBMDbContext _dbContext;

    public InventoryRepository(NVBMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InventoryLevel?> GetInventoryLevelAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _dbContext.InventoryLevels
            .FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
    }

    public async Task<ProductUom?> GetProductUomAsync(Guid productId, Guid uomId, CancellationToken cancellationToken)
    {
        return await _dbContext.ProductUoms
            .FirstOrDefaultAsync(u => u.Id == uomId && u.ProductId == productId, cancellationToken);
    }

    public void UpdateInventoryRowVersion(InventoryLevel inventory, byte[] originalRowVersion)
    {
        _dbContext.Entry(inventory).Property(i => i.RowVersion).OriginalValue = originalRowVersion;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
