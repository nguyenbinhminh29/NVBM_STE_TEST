using MediatR;
using NVBM.Application.DTOs;

namespace NVBM.Application.Features.Inventory.Commands;

public record UpdateInventoryCommand(
    Guid ProductId,
    decimal Delta,
    byte[] RowVersion
) : IRequest<APIResponse<bool>>;

public record ReserveInventoryCommand(
    Guid ProductId,
    Guid UomId,
    decimal Quantity,
    string IdempotencyKey
) : IRequest<APIResponse<bool>>;
