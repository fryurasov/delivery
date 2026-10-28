using Ddd;

namespace DeliveryApp.Core.Domain.Models.Order.DomainEvents;

public sealed class OrderCompletedDomainEvent : DomainEvent
{
    /// <summary>
    /// For serialization
    /// </summary>
    public OrderCompletedDomainEvent()
    {
    }

    /// <summary>
    /// For domain
    /// </summary>
    public OrderCompletedDomainEvent(OrderAggregate order)
    {
        OrderId = order.Id;
    }

    public Guid OrderId { get; private set; }
}
