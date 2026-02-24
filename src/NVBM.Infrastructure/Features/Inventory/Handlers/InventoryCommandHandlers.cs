using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NVBM.Application.DTOs;
using NVBM.Application.Features.Inventory.Commands;
using NVBM.Infrastructure.Data;

namespace NVBM.Infrastructure.Features.Inventory.Handlers;

public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, APIResponse<bool>>
{
    private readonly NVBMDbContext _dbContext;

    public UpdateInventoryCommandHandler(NVBMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<APIResponse<bool>> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _dbContext.InventoryLevels
            .FirstOrDefaultAsync(i => i.ProductId == request.ProductId, cancellationToken);

        if (inventory == null)
            return APIResponse<bool>.Fail("Inventory record not found.");

        _dbContext.Entry(inventory).Property(i => i.RowVersion).OriginalValue = request.RowVersion;

        inventory.AvailableQuantity += request.Delta;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return APIResponse<bool>.Ok(true, "Inventory updated successfully.");
    }
}

public class ReserveInventoryCommandHandler : IRequestHandler<ReserveInventoryCommand, APIResponse<bool>>
{
    private readonly NVBMDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public ReserveInventoryCommandHandler(NVBMDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<APIResponse<bool>> Handle(ReserveInventoryCommand request, CancellationToken cancellationToken)
    {
        // 1. Check Idempotency Key
        var idempotencyKey = $"reserve:idempotency:{request.IdempotencyKey}";
        var existingRequest = await _cache.GetStringAsync(idempotencyKey, cancellationToken);
        if (!string.IsNullOrEmpty(existingRequest))
        {
            return APIResponse<bool>.Ok(true, "Request already processed (Idempotent).");
        }

        // 2. Process Reserve & Multi-UOM Logic
        var productUom = await _dbContext.ProductUoms
            .FirstOrDefaultAsync(u => u.Id == request.UomId && u.ProductId == request.ProductId, cancellationToken);
            
        if (productUom == null)
            return APIResponse<bool>.Fail("Invalid UOM for this product.");
            
        var baseQuantityToReserve = request.Quantity * productUom.ConversionFactor;

        var inventory = await _dbContext.InventoryLevels
            .FirstOrDefaultAsync(i => i.ProductId == request.ProductId, cancellationToken);

        if (inventory == null)
            return APIResponse<bool>.Fail("Inventory record not found.");

        if (inventory.AvailableQuantity < baseQuantityToReserve)
            return APIResponse<bool>.Fail("Not enough available quantity.");

        inventory.AvailableQuantity -= baseQuantityToReserve;
        inventory.ReservedQuantity += baseQuantityToReserve;

        await _dbContext.SaveChangesAsync(cancellationToken);

        // 3. Save Idempotency Key for 24 hours
        await _cache.SetStringAsync(idempotencyKey, "processed", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        }, cancellationToken);

        return APIResponse<bool>.Ok(true, "Reserved successfully.");
    }
}
