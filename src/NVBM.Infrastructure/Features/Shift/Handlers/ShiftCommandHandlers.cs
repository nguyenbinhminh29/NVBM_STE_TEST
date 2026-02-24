using NVBM.Application.DTOs;
using NVBM.Application.Features.Shift.Commands;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;

namespace NVBM.Infrastructure.Features.Shift.Handlers;

public class ShiftCommandHandler
{
    public async Task<APIResponse<ShiftResponseDto>> Handle(OpenShiftCommand command, IShiftRepository repository, CancellationToken cancellationToken)
    {
        var existingShiftDevice = await repository.GetOpenShiftAsync(command.Request.PosDeviceId, cancellationToken);
        if (existingShiftDevice != null)
        {
            return APIResponse<ShiftResponseDto>.Fail($"Device {command.Request.PosDeviceId} already has an OPEN shift.");
        }

        var hasOpenUserShift = await repository.HasOpenShiftByUserAsync(command.Request.UserId, cancellationToken);
        if (hasOpenUserShift)
        {
            return APIResponse<ShiftResponseDto>.Fail($"User {command.Request.UserId} already has an OPEN shift on another device.");
        }

        var shift = new NVBM.Domain.Entities.Shift
        {
            Id = Guid.NewGuid(),
            UserId = command.Request.UserId,
            PosDeviceId = command.Request.PosDeviceId,
            StartingCash = command.Request.StartingCash,
            Status = "OPEN",
            OpenTime = DateTime.UtcNow
        };

        repository.AddShift(shift);
        await repository.SaveChangesAsync(cancellationToken);

        return APIResponse<ShiftResponseDto>.Ok(new ShiftResponseDto
        {
            Id = shift.Id,
            PosDeviceId = shift.PosDeviceId,
            OpenTime = shift.OpenTime,
            StartingCash = shift.StartingCash,
            Status = shift.Status
        }, "Shift opened successfully.");
    }

    public async Task<APIResponse<bool>> Handle(AddShiftTransactionCommand command, IShiftRepository repository, CancellationToken cancellationToken)
    {
        var shift = await repository.GetShiftByIdAsync(command.ShiftId, cancellationToken);
        
        if (shift == null || shift.Status != "OPEN")
        {
            return APIResponse<bool>.Fail("Shift not found or not in OPEN status.");
        }

        var transaction = new ShiftTransaction
        {
            Id = Guid.NewGuid(),
            ShiftId = shift.Id,
            TransactionType = command.Request.TransactionType, // PAY_IN or PAY_OUT
            Amount = command.Request.Amount,
            Reason = command.Request.Reason,
            TransactionTime = DateTime.UtcNow
        };

        repository.AddTransaction(transaction);
        await repository.SaveChangesAsync(cancellationToken);

        return APIResponse<bool>.Ok(true, "Transaction added successfully.");
    }

    public async Task<APIResponse<bool>> Handle(CloseShiftCommand command, IShiftRepository repository, CancellationToken cancellationToken)
    {
        var shift = await repository.GetShiftByIdAsync(command.ShiftId, cancellationToken);
        
        if (shift == null || shift.Status != "OPEN")
        {
            return APIResponse<bool>.Fail("Shift not found or already CLOSED.");
        }

        // Logic Reconcile: Calculate Expected Cash using CQRS fields and Opening balance
        var totalPayIns = shift.Transactions.Where(t => t.TransactionType == "PAY_IN").Sum(t => t.Amount);
        var totalPayOuts = shift.Transactions.Where(t => t.TransactionType == "PAY_OUT").Sum(t => t.Amount);

        // Expected Cash = Starting + TotalCashReceived (from Orders) + PayIns - PayOuts
        shift.ExpectedCash = shift.StartingCash + shift.TotalCashReceived + totalPayIns - totalPayOuts;
        
        shift.ActualCash = command.Request.ActualCash;
        shift.Status = "CLOSED";
        shift.CloseTime = DateTime.UtcNow;

        var difference = shift.ActualCash.Value - shift.ExpectedCash;

        await repository.SaveChangesAsync(cancellationToken);

        if (difference != 0)
        {
            return APIResponse<bool>.Ok(true, $"Shift closed with a difference of {difference}. Expected: {shift.ExpectedCash}, Actual: {shift.ActualCash}");
        }

        return APIResponse<bool>.Ok(true, "Shift closed successfully. Perfectly balanced.");
    }
}
