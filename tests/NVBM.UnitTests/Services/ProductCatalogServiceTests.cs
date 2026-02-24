using Moq;
using NVBM.Application.DTOs;
using NVBM.Application.Interfaces;
using NVBM.Domain.Entities;
using NVBM.Infrastructure.Services;
using Xunit;

namespace NVBM.UnitTests.Services;

public class ProductCatalogServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly ProductCatalogService _service;

    public ProductCatalogServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _service = new ProductCatalogService(_repositoryMock.Object, _eventPublisherMock.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldAddProductAndPublishEvent()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Sku = "TEST-SKU",
            Name = "Test Product",
            Description = "Test Description",
            CategoryId = Guid.NewGuid(),
            Uoms = new List<CreateProductUomDto>
            {
                new CreateProductUomDto { UomCode = "EA", UomName = "Each", Price = 10.0m }
            },
            Attributes = new List<CreateProductAttributeDto>
            {
                new CreateProductAttributeDto { Key = "Color", Value = "Red" }
            }
        };

        // Act
        var result = await _service.CreateProductAsync(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisherMock.Verify(e => e.PublishProductChangedEventAsync(result, "Created"), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldSoftDeleteProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, IsActive = true };
        _repositoryMock.Setup(r => r.GetProductByIdAsync(productId, It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(product);

        // Act
        var result = await _service.DeleteProductAsync(productId);

        // Assert
        Assert.True(result);
        Assert.False(product.IsActive);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisherMock.Verify(e => e.PublishProductChangedEventAsync(productId, "Deleted"), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldReturnFalse_WhenProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetProductByIdAsync(productId, It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.DeleteProductAsync(productId);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
