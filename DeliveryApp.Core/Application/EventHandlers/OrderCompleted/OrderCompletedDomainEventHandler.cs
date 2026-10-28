using DeliveryApp.Core.Domain.Models.Order.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.EventHandlers.OrderCompleted;

public sealed class OrderCompletedDomainEventHandler : INotificationHandler<OrderCompletedDomainEvent>
{
    private readonly IOrderEventsProducer _producer;

    public OrderCompletedDomainEventHandler(IOrderEventsProducer producer)
    {
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
    }

    public async Task Handle(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _producer.PublishOrderCompletedAsync(notification, cancellationToken);
    }
}
