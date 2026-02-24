using Moq;
using NVBM.Application.DTOs;
using NVBM.Application.Features.Shift.Commands;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Features.Shift.Handlers;
using Xunit;

namespace NVBM.UnitTests.Handlers;

public class ShiftCommandHandlersTests
{
    private readonly Mock<IShiftRepository> _repositoryMock;
    private readonly ShiftCommandHandlers _handler;

    public ShiftCommandHandlersTests()
    {
        _repositoryMock = new Mock<IShiftRepository>();
        _handler = new ShiftCommandHandlers();
    }

    [Fact]
    public async Task OpenShift_ShouldReturnOk_WhenCompatible()
    {
        // Arrange
        var request = new OpenShiftRequest
        {
            UserId = Guid.NewGuid(),
            PosDeviceId = "POS-1",
            StartingCash = 1000.0m
        };
        var command = new OpenShiftCommand(request);

        _repositoryMock.Setup(r => r.GetOpenShiftAsync(request.PosDeviceId, It.IsAny<CancellationToken>())).ReturnsAsync((Shift?)null);
        _repositoryMock.Setup(r => r.HasOpenShiftByUserAsync(request.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, _repositoryMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("OPEN", result.Data!.Status);
        _repositoryMock.Verify(r => r.AddShift(It.IsAny<Shift>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OpenShift_ShouldFail_WhenDeviceHasOpenShift()
    {
        // Arrange
        var request = new OpenShiftRequest
        {
            UserId = Guid.NewGuid(),
            PosDeviceId = "POS-1",
            StartingCash = 1000.0m
        };
        var command = new OpenShiftCommand(request);

        _repositoryMock.Setup(r => r.GetOpenShiftAsync(request.PosDeviceId, It.IsAny<CancellationToken>())).ReturnsAsync(new Shift());

        // Act
        var result = await _handler.Handle(command, _repositoryMock.Object, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        _repositoryMock.Verify(r => r.AddShift(It.IsAny<Shift>()), Times.Never);
    }

    [Fact]
    public async Task CloseShift_ShouldCalculateExpectedCashCorrectly()
    {
        // Arrange
        var shiftId = Guid.NewGuid();
        var shift = new Shift
        {
            Id = shiftId,
            Status = "OPEN",
            StartingCash = 100.0m,
            TotalCashReceived = 500.0m, // From orders
            Transactions = new List<ShiftTransaction>
            {
                new ShiftTransaction { TransactionType = "PAY_IN", Amount = 50.0m },
                new ShiftTransaction { TransactionType = "PAY_OUT", Amount = 20.0m }
            }
        };

        var request = new CloseShiftRequest { ActualCash = 630.0m };
        var command = new CloseShiftCommand(shiftId, request);

        _repositoryMock.Setup(r => r.GetShiftByIdAsync(shiftId, It.IsAny<CancellationToken>())).ReturnsAsync(shift);

        // Expected = 100 + 500 + 50 - 20 = 630

        // Act
        var result = await _handler.Handle(command, _repositoryMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("CLOSED", shift.Status);
        Assert.Equal(630.0m, shift.ExpectedCash);
        Assert.Contains("Perfectly balanced", result.Message!);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
