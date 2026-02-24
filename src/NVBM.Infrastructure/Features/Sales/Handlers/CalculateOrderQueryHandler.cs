using NVBM.Application.DTOs;
using NVBM.Application.Features.Sales.Queries;
using NVBM.Application.Interfaces;
using Wolverine;

namespace NVBM.Infrastructure.Features.Sales.Handlers;

public class CalculateOrderQueryHandler
{
    public async Task<APIResponse<OrderCalculateResponse>> Handle(CalculateOrderQuery query, IPromotionEngine engine, CancellationToken cancellationToken)
    {
        try
        {
            var result = await engine.CalculateOrderAsync(query.Request, cancellationToken);
            return APIResponse<OrderCalculateResponse>.Ok(result, "Order calculated successfully with applying promotions.");
        }
        catch (Exception ex)
        {
            return APIResponse<OrderCalculateResponse>.Fail($"Failed to calculate order: {ex.Message}");
        }
    }
}
