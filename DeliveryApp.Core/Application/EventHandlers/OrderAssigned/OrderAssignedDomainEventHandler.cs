using DeliveryApp.Core.Domain.Models.Order.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.EventHandlers.OrderAssigned;

public sealed class OrderAssignedDomainEventHandler : INotificationHandler<OrderAssignedDomainEvent>
{
    private readonly IOrderEventsProducer _producer;

    public OrderAssignedDomainEventHandler(IOrderEventsProducer producer)
    {
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
    }

    public async Task Handle(OrderAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _producer.PublishOrderAssignedAsync(notification, cancellationToken);
    }
}
