using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

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
    ///     Factory Method
    /// </summary>
    /// <param name="basketId">Идентификатор корзины</param>
    /// <param name="location">Координата заказа на доске</param>
    /// <param name="volume">Объем заказа</param>
    public static Result<OrderAggregate, Error> Create(Guid basketId, LocationVo location, VolumeVo volume)
    {
        if (basketId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(basketId));
        
        return new OrderAggregate(basketId, location, volume);
    }
    
    /// <summary>
    ///     Переводит заказ в новый статус
    /// </summary>
    /// <param name="newStatus">Новый статус</param>
    public UnitResult<Error> ChangeStatus(OrderStatusVo newStatus)
    {
        var canTransition = OrderStatusVo.EnsureCanTransitionTo(Status, newStatus);
        if (canTransition.IsFailure)
            return canTransition.Error;

        Status = newStatus;
        
        return UnitResult.Success<Error>();
    }
}