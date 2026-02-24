using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Services;
using Xunit;

namespace NVBM.UnitTests.Services;

public class PromotionEngineTests
{
    private readonly Mock<IPromotionRepository> _promotionRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly IMemoryCache _cache;
    private readonly PromotionEngine _engine;

    public PromotionEngineTests()
    {
        _promotionRepositoryMock = new Mock<IPromotionRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _engine = new PromotionEngine(_promotionRepositoryMock.Object, _productRepositoryMock.Object, _cache);
    }

    [Fact]
    public async Task CalculateOrderAsync_ShouldApplyPromotion_WhenEligible()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var uomId = Guid.NewGuid();
        var request = new OrderCalculateRequest
        {
            Items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = productId, UomId = uomId, Quantity = 5 }
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Uoms = new List<ProductUom>
            {
                new ProductUom { Id = uomId, UomName = "Unit", Price = 100.0m }
            }
        };

        var promotion = new Promotion
        {
            Id = Guid.NewGuid(),
            Name = "Buy 5 Get 10 Off",
            IsActive = true,
            Rules = new List<PromotionRule>
            {
                new PromotionRule
                {
                    RuleType = "MIN_ITEM_QTY",
                    RulePayload = JsonSerializer.Serialize(new { productId = productId, minQuantity = 5 })
                }
            },
            Actions = new List<PromotionAction>
            {
                new PromotionAction
                {
                    ActionType = "ITEM_FIXED_DISCOUNT",
                    ActionPayload = JsonSerializer.Serialize(new { targetProductId = productId, discountValue = 10.0m })
                }
            }
        };

        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(productId, It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _promotionRepositoryMock.Setup(r => r.GetActivePromotionsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Promotion> { promotion });

        // Act
        var result = await _engine.CalculateOrderAsync(request);

        // Assert
        Assert.Equal(500.0m, result.SubTotal);
        Assert.Equal(10.0m, result.TotalDiscount);
        Assert.Equal(490.0m, result.GrandTotal);
        Assert.Single(result.Lines[0].AppliedPromotions);
        Assert.Equal("Buy 5 Get 10 Off", result.Lines[0].AppliedPromotions[0]);
    }

    [Fact]
    public async Task CalculateOrderAsync_ShouldNotApplyPromotion_WhenNotEligible()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var uomId = Guid.NewGuid();
        var request = new OrderCalculateRequest
        {
            Items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = productId, UomId = uomId, Quantity = 3 }
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Uoms = new List<ProductUom>
            {
                new ProductUom { Id = uomId, UomName = "Unit", Price = 100.0m }
            }
        };

        var promotion = new Promotion
        {
            Id = Guid.NewGuid(),
            Name = "Buy 5 Get 10 Off",
            IsActive = true,
            Rules = new List<PromotionRule>
            {
                new PromotionRule
                {
                    RuleType = "MIN_ITEM_QTY",
                    RulePayload = JsonSerializer.Serialize(new { productId = productId, minQuantity = 5 })
                }
            },
            Actions = new List<PromotionAction>
            {
                new PromotionAction
                {
                    ActionType = "ITEM_FIXED_DISCOUNT",
                    ActionPayload = JsonSerializer.Serialize(new { targetProductId = productId, discountValue = 10.0m })
                }
            }
        };

        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(productId, It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _promotionRepositoryMock.Setup(r => r.GetActivePromotionsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Promotion> { promotion });

        // Act
        var result = await _engine.CalculateOrderAsync(request);

        // Assert
        Assert.Equal(300.0m, result.SubTotal);
        Assert.Equal(0.0m, result.TotalDiscount);
        Assert.Equal(300.0m, result.GrandTotal);
        Assert.Empty(result.Lines[0].AppliedPromotions);
    }
}
