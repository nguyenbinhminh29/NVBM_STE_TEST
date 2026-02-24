using NVBM.Domain.Entities;

namespace NVBM.Application.Interfaces;

public interface IPromotionRepository
{
    Task<List<Promotion>> GetActivePromotionsAsync(DateTime currentDate, CancellationToken cancellationToken = default);
}
