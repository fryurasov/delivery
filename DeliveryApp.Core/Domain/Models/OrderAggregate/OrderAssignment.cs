using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

/// <summary>
///     Назначение заказа на курьера
/// </summary>
public class OrderAssignmentEntity : Entity<Guid>
{
    /// <summary>
    ///     Ctr
    /// </summary>
    /// <param name="orderId">Идентификатор заказа</param>
    /// <param name="location">Координата заказа на доске</param>
    /// <param name="volume">Объем заказа</param>
    private OrderAssignmentEntity(Guid orderId, LocationVo location, VolumeVo volume) : base(Guid.NewGuid()) {
        OrderId = orderId;
        Location = location;
        Volume = volume;
        Status = OrderStatusVo.Assigned;
    }
    
    /// <summary>
    ///     Идентификатор заказа
    /// </summary>
    public Guid OrderId { get; private set; }
    
    /// <summary>
    ///     Объем заказа
    /// </summary>
    public VolumeVo Volume { get; private set; } 
    
    /// <summary>
    ///     Координата заказа на доске
    /// </summary>
    public LocationVo Location { get; private set; } 
    
    /// <summary>
    ///     Статус назначения
    /// </summary>
    public OrderStatusVo Status { get; private set; }
    
    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="orderId">Идентификатор заказа</param>
    /// <param name="location">Координата заказа на доске</param>
    /// <param name="volume">Объем заказа</param>
    /// <returns>Результат</returns>
    public static Result<OrderAssignmentEntity, Error> Create(Guid orderId, LocationVo location, VolumeVo volume)
    {
        if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
        return new OrderAssignmentEntity(orderId, location, volume);
    }

    /// <summary>
    ///     Проверяет, может ли курьер завершить назначение, в зависимости от его местоположения.
    /// </summary>
    /// <param name="courierLocation">Позиция курьера</param>
    /// <returns> true, если курьер находится в той же клетке, что и заказ. Иначе false. </returns>
    public bool CanComplete(LocationVo courierLocation)
    {
        var distance = LocationVo.GetDistance(Location, courierLocation);
        
        return distance == 0; 
    }
    
    /// <summary>
    ///     Завершить заказ
    /// </summary>
    /// <param name="courierLocation">Позиция курьера</param>
    /// <returns>Результат</returns>
    public UnitResult<Error> Complete(LocationVo courierLocation)
    {
        if (!CanComplete(courierLocation))
            return Errors.CourierTooFar();

        Status = OrderStatusVo.Completed;
        
        return UnitResult.Success<Error>();
    }
    
    public static class Errors
    {
        public static Error CourierTooFar()
        {
            return new Error(
                $"{nameof(OrderAssignmentEntity).ToLowerInvariant()}.courier.too.far",
                "Courier is too far from order location");
        }
    }
}
