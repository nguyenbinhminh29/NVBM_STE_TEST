namespace NVBM.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishProductChangedEventAsync(Guid productId, string changeType);
}
