using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;

namespace DeliveryApp.Core.Domain.Models.Order;

public class OrderAggregate : Aggregate<Guid>, IAggregateRoot
{
    /// <summary>
    ///     Ctr
    /// </summary>
    /// <param name="id">Идентификатор корзины</param>
    /// <param name="location">Координата заказа на доске</param>
    /// <param name="volume">Объем заказа</param>
    private OrderAggregate(Guid id, LocationVo location, VolumeVo volume) : base(id) {
        Location = location;
        Volume = volume;
        Status = OrderStatusVo.Created;
    }
    
    /// <summary>
    ///     Объем заказа
    /// </summary>
    public VolumeVo Volume { get; private set; }
    
    /// <summary>
    ///     Координата заказа на доске
    /// </summary>
    public LocationVo Location { get; private set; }
    
    /// <summary>
    ///     Статус заказа
    /// </summary>
    public OrderStatusVo Status { get; private set; }
    
    /// <summary>
    ///     Статус заказа
    /// </summary>
    public Guid? CourierId { get; private set; }

    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="basketId">Идентификатор корзины</param>
    /// <param name="location">Координата заказа на доске</param>
    /// <param name="volume">Объем заказа</param>
    public static Result<OrderAggregate, Error> Create(Guid basketId, LocationVo location, VolumeVo volume)
    {
        if (basketId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(basketId));
        if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
        if (volume == null) return GeneralErrors.ValueIsRequired(nameof(volume));
        
        return new OrderAggregate(basketId, location, volume);
    }
    
    /// <summary>
    ///     Назначить заказ
    /// </summary>
    /// <returns>Результат</returns>
    public UnitResult<Error> Assign(CourierAggregate courier)
    {
        if (courier == null) return GeneralErrors.ValueIsRequired(nameof(courier));
        
        var takeOrderResult = courier.AssignOrder(this);
        if (takeOrderResult.IsFailure)
            return takeOrderResult.Error;
        
        CourierId = courier.Id;
        Status = OrderStatusVo.Assigned;
        
        return UnitResult.Success<Error>();
    }

    /// <summary>
    ///     Завершить заказ
    /// </summary>
    /// <returns>Результат</returns>
    public UnitResult<Error> Complete()
    {
        var transitionResult = Status.EnsureCanTransitionTo(OrderStatusVo.Completed);
        if (transitionResult.IsFailure)
            return transitionResult;

        Status = OrderStatusVo.Completed;

        return UnitResult.Success<Error>();
    }
}
