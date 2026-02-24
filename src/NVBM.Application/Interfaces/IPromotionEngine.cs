using NVBM.Application.DTOs;

namespace NVBM.Application.Interfaces;

public interface IPromotionEngine
{
    Task<OrderCalculateResponse> CalculateOrderAsync(OrderCalculateRequest request, CancellationToken cancellationToken = default);
}
