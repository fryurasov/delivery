using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

/// <summary>
///     Статус назначеного заказа
/// </summary>
public sealed class OrderStatusVo : ValueObject
{
    public static OrderStatusVo Assigned => new(nameof(Assigned));
    public static OrderStatusVo Completed => new(nameof(Completed));
    
    public string Name { get; private set; }
    
    private OrderStatusVo(string name)
    {
        Name = name;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}