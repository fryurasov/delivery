using DeliveryApp.Core.Domain.Models.Order.DomainEvents;

namespace DeliveryApp.Core.Ports;

public interface IOrderEventsProducer
{
    Task PublishOrderAssignedAsync(OrderAssignedDomainEvent notification, CancellationToken cancellationToken);
    Task PublishOrderCompletedAsync(OrderCompletedDomainEvent notification, CancellationToken cancellationToken);
}
