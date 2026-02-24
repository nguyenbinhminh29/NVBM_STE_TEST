using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NVBM.Application.DTOs;
using NVBM.Application.Features.Inventory.Commands;
using NVBM.Application.Interfaces;
using NVBM.Infrastructure.Data;
using Wolverine;

namespace NVBM.Infrastructure.Features.Inventory.Handlers;

public class UpdateInventoryCommandHandler
{
    public async Task<APIResponse<bool>> Handle(UpdateInventoryCommand request, IInventoryRepository repository, CancellationToken cancellationToken)
    {
        var inventory = await repository.GetInventoryLevelAsync(request.ProductId, cancellationToken);

        if (inventory == null)
            return APIResponse<bool>.Fail("Inventory record not found.");

        repository.UpdateInventoryRowVersion(inventory, request.RowVersion);

        inventory.AvailableQuantity += request.Delta;

        await repository.SaveChangesAsync(cancellationToken);

        return APIResponse<bool>.Ok(true, "Inventory updated successfully.");
    }
}

public class ReserveInventoryCommandHandler
{
    public async Task<APIResponse<bool>> Handle(ReserveInventoryCommand request, IInventoryRepository repository, IDistributedCache cache, CancellationToken cancellationToken)
    {
        // 1. Check Idempotency Key
        var idempotencyKey = $"reserve:idempotency:{request.IdempotencyKey}";
        var existingRequest = await cache.GetStringAsync(idempotencyKey, cancellationToken);
        if (!string.IsNullOrEmpty(existingRequest))
        {
            return APIResponse<bool>.Ok(true, "Request already processed (Idempotent).");
        }

        // 2. Process Reserve & Multi-UOM Logic
        var productUom = await repository.GetProductUomAsync(request.ProductId, request.UomId, cancellationToken);
            
        if (productUom == null)
            return APIResponse<bool>.Fail("Invalid UOM for this product.");
            
        var baseQuantityToReserve = request.Quantity * productUom.ConversionFactor;

        var inventory = await repository.GetInventoryLevelAsync(request.ProductId, cancellationToken);

        if (inventory == null)
            return APIResponse<bool>.Fail("Inventory record not found.");

        if (inventory.AvailableQuantity < baseQuantityToReserve)
            return APIResponse<bool>.Fail("Not enough available quantity.");

        inventory.AvailableQuantity -= baseQuantityToReserve;
        inventory.ReservedQuantity += baseQuantityToReserve;

        await repository.SaveChangesAsync(cancellationToken);

        // 3. Save Idempotency Key for 24 hours
        await cache.SetStringAsync(idempotencyKey, "processed", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        }, cancellationToken);

        return APIResponse<bool>.Ok(true, "Reserved successfully.");
    }
}
