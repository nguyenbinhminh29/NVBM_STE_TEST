using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NVBM.Application.DTOs;
using NVBM.Application.Features.Inventory.Commands;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Features.Inventory.Handlers;
using Xunit;

namespace NVBM.UnitTests.Handlers;

public class InventoryCommandHandlersTests
{
    private readonly Mock<IInventoryRepository> _repositoryMock;
    private readonly Mock<IDistributedCache> _cacheMock;

    public InventoryCommandHandlersTests()
    {
        _repositoryMock = new Mock<IInventoryRepository>();
        _cacheMock = new Mock<IDistributedCache>();
    }

    [Fact]
    public async Task UpdateInventoryCommandHandler_ShouldUpdateQuantity()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new UpdateInventoryCommand(productId, 10, new byte[] { 1, 2, 3 });
        var handler = new UpdateInventoryCommandHandler();
        var inventory = new InventoryLevel { ProductId = productId, AvailableQuantity = 100 };

        _repositoryMock.Setup(r => r.GetInventoryLevelAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(inventory);

        // Act
        var result = await handler.Handle(request, _repositoryMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(110, inventory.AvailableQuantity);
        _repositoryMock.Verify(r => r.UpdateInventoryRowVersion(inventory, request.RowVersion), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReserveInventoryCommandHandler_ShouldReserveQuantityWithUomFactor()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var uomId = Guid.NewGuid();
        var request = new ReserveInventoryCommand(productId, uomId, 2, "idemp-1");
        var handler = new ReserveInventoryCommandHandler();
        
        var productUom = new ProductUom { Id = uomId, ConversionFactor = 12.0m }; // Case of 12
        var inventory = new InventoryLevel { ProductId = productId, AvailableQuantity = 100, ReservedQuantity = 0 };

        _repositoryMock.Setup(r => r.GetProductUomAsync(productId, uomId, It.IsAny<CancellationToken>())).ReturnsAsync(productUom);
        _repositoryMock.Setup(r => r.GetInventoryLevelAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(inventory);
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);

        // Act
        var result = await handler.Handle(request, _repositoryMock.Object, _cacheMock.Object, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        // 2 * 12 = 24
        Assert.Equal(76, inventory.AvailableQuantity);
        Assert.Equal(24, inventory.ReservedQuantity);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReserveInventoryCommandHandler_ShouldReturnFail_WhenStockNotEnough()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var uomId = Guid.NewGuid();
        var request = new ReserveInventoryCommand(productId, uomId, 10, "idemp-2");
        var handler = new ReserveInventoryCommandHandler();
        
        var productUom = new ProductUom { Id = uomId, ConversionFactor = 1.0m };
        var inventory = new InventoryLevel { ProductId = productId, AvailableQuantity = 5, ReservedQuantity = 0 };

        _repositoryMock.Setup(r => r.GetProductUomAsync(productId, uomId, It.IsAny<CancellationToken>())).ReturnsAsync(productUom);
        _repositoryMock.Setup(r => r.GetInventoryLevelAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(inventory);

        // Act
        var result = await handler.Handle(request, _repositoryMock.Object, _cacheMock.Object, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Not enough available quantity.", result.Message);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
