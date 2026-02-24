using NVBM.Application.DTOs;
using NVBM.Domain.Entities;

namespace NVBM.Application.Interfaces;

public interface IShiftRepository
{
    Task<Shift?> GetOpenShiftAsync(string posDeviceId, CancellationToken cancellationToken = default);
    Task<Shift?> GetShiftByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasOpenShiftByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    void AddShift(Shift shift);
    void AddTransaction(ShiftTransaction transaction);
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
