using Ddd;

namespace DeliveryApp.Core.Domain.Models.Order.DomainEvents;

public sealed class OrderAssignedDomainEvent : DomainEvent
{
    /// <summary>
    /// For serialization
    /// </summary>
    public OrderAssignedDomainEvent()
    {
    }

    /// <summary>
    /// For domain
    /// </summary>
    public OrderAssignedDomainEvent(OrderAggregate order)
    {
        OrderId = order.Id;
    }

    public Guid OrderId { get; private set; }
}
