using CSharpFunctionalExtensions;
using Errs;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

/// <summary>
///     Статус назначеного заказа
/// </summary>
public sealed class OrderStatusVo : ValueObject
{
    public static OrderStatusVo Assigned => new(OrderStatusEnum.Assigned);
    public static OrderStatusVo Completed => new(OrderStatusEnum.Completed);
    
    public OrderStatusEnum Status { get; private set; }

    public bool IsFinal()
    {
        return Status.Equals(OrderStatusEnum.Completed);
    }
    
    private OrderStatusVo(OrderStatusEnum status)
    {
        Status = status;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Status;
    }

    public enum OrderStatusEnum
    {
        Assigned,
        Completed
    }
}
