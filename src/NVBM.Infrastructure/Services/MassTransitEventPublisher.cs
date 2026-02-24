using Wolverine;
using NVBM.Application.Interfaces;

namespace NVBM.Infrastructure.Services;

public record ProductChangedEvent(Guid ProductId, string ChangeType);

public class WolverineEventPublisher : IEventPublisher
{
    private readonly IMessageBus _bus;

    public WolverineEventPublisher(IMessageBus bus)
    {
        _bus = bus;
    }

    public async Task PublishProductChangedEventAsync(Guid productId, string changeType)
    {
        await _bus.PublishAsync(new ProductChangedEvent(productId, changeType));
    }
}
