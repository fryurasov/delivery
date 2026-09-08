using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

/// <summary>
///     Назначение заказа на курьера
/// </summary>
public class CourierAssignmentEntity : Entity<Guid>
{
    /// <summary>
    ///     Ctr
    /// </summary>
    /// <param name="orderId">Идентификатор заказа</param>
    /// <param name="location">Координата заказа на доске</param>
    /// <param name="volume">Объем заказа</param>
    private CourierAssignmentEntity(Guid orderId, LocationVo location, VolumeVo volume) : base(Guid.NewGuid()) {
        
        OrderId = orderId;
        Location = location;
        Volume = volume;
        Status = CourierAssignmentStatusVo.Assigned;
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
    public CourierAssignmentStatusVo Status { get; private set; }
    
    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="orderId">Идентификатор заказа</param>
    /// <param name="location">Координата заказа на доске</param>
    /// <param name="volume">Объем заказа</param>
    /// <returns>Результат</returns>
    public static Result<CourierAssignmentEntity, Error> Create(Guid orderId, LocationVo location, VolumeVo volume)
    {
        return new CourierAssignmentEntity(orderId, location, volume);
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

        Status = CourierAssignmentStatusVo.Completed;
        
        return UnitResult.Success<Error>();
    }
    
    public static class Errors
    {
        public static Error CourierTooFar()
        {
            return new Error(
                $"{nameof(CourierAssignmentEntity).ToLowerInvariant()}.courier.too.far",
                "Courier is too far from order location");
        }
    }
}
