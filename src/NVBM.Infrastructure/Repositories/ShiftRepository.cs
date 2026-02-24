using Microsoft.EntityFrameworkCore;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Data;

namespace NVBM.Infrastructure.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly NVBMDbContext _dbContext;

    public ShiftRepository(NVBMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Shift?> GetOpenShiftAsync(string posDeviceId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Shifts
            .FirstOrDefaultAsync(s => s.PosDeviceId == posDeviceId && s.Status == "OPEN", cancellationToken);
    }

    public async Task<Shift?> GetShiftByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Shifts
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> HasOpenShiftByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Shifts
            .AnyAsync(s => s.UserId == userId && s.Status == "OPEN", cancellationToken);
    }

    public void AddShift(Shift shift)
    {
        _dbContext.Shifts.Add(shift);
    }

    public void AddTransaction(ShiftTransaction transaction)
    {
        _dbContext.ShiftTransactions.Add(transaction);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
