using MassTransit;
using NVBM.Application.Interfaces;

namespace NVBM.Infrastructure.Services;

public record ProductChangedEvent(Guid ProductId, string ChangeType);

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishProductChangedEventAsync(Guid productId, string changeType)
    {
        await _publishEndpoint.Publish(new ProductChangedEvent(productId, changeType));
    }
}
