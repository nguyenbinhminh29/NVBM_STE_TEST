using FluentValidation;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using Wolverine;

namespace NVBM.Application.Features.Shift.Commands;

public record OpenShiftCommand(OpenShiftRequest Request);

public class OpenShiftCommandValidator : AbstractValidator<OpenShiftCommand>
{
    public OpenShiftCommandValidator()
    {
        RuleFor(x => x.Request.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.Request.PosDeviceId).NotEmpty().WithMessage("PosDeviceId is required.");
        RuleFor(x => x.Request.StartingCash).GreaterThanOrEqualTo(0).WithMessage("StartingCash cannot be negative.");
    }
}

public record AddShiftTransactionCommand(Guid ShiftId, ShiftTransactionRequest Request);

public class AddShiftTransactionCommandValidator : AbstractValidator<AddShiftTransactionCommand>
{
    public AddShiftTransactionCommandValidator()
    {
        RuleFor(x => x.ShiftId).NotEmpty().WithMessage("ShiftId is required.");
        RuleFor(x => x.Request.TransactionType)
            .Must(t => t == "PAY_IN" || t == "PAY_OUT")
            .WithMessage("TransactionType must be PAY_IN or PAY_OUT.");
        RuleFor(x => x.Request.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0.");
    }
}

public record CloseShiftCommand(Guid ShiftId, CloseShiftRequest Request);

public class CloseShiftCommandValidator : AbstractValidator<CloseShiftCommand>
{
    public CloseShiftCommandValidator()
    {
        RuleFor(x => x.ShiftId).NotEmpty().WithMessage("ShiftId is required.");
        RuleFor(x => x.Request.ActualCash).GreaterThanOrEqualTo(0).WithMessage("ActualCash cannot be negative.");
    }
}
