using Microsoft.EntityFrameworkCore;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Data;

namespace NVBM.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly NVBMDbContext _dbContext;

    public PromotionRepository(NVBMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Promotion>> GetActivePromotionsAsync(DateTime currentDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Promotions
            .Include(p => p.Rules)
            .Include(p => p.Actions)
            .AsNoTracking()
            .Where(p => p.IsActive && p.StartDate <= currentDate && p.EndDate >= currentDate)
            .ToListAsync(cancellationToken);
    }
}
