using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;
using Errs.Extensions;

namespace DeliveryApp.Core.Domain.Models.Courier;

public class CourierAggregate : Aggregate<Guid>, IAggregateRoot 
{
    /// <summary>
    ///     Ctr
    /// </summary>
    /// <param name="name">Имя курьера</param>
    /// <param name="location">Координата заказа на доске</param>
    private CourierAggregate(string name, LocationVo location) : base(Guid.NewGuid()) {
        Name = name;
        Location = location;
    }
    
    /// <summary>
    ///     Имя
    /// </summary>
    public string Name { get; private set; } 
    
    /// <summary>
    ///     Позиция на доске
    /// </summary>
    public LocationVo Location  { get; private set; }
    
    /// <summary>
    ///     Максимальный объем заказов
    /// </summary>
    public VolumeVo VolumeMax => VolumeVo.CourierVolumeMax;
    
    /// <summary>
    ///     Назначения заказов
    /// </summary>
    private readonly List<CourierAssignmentEntity> _assignments = new();
    public IReadOnlyCollection<CourierAssignmentEntity> AssignmentsAsReadOnly => _assignments.AsReadOnly();
    
    /// <summary>
    ///     Ctr
    /// </summary>
    /// <param name="name">Имя курьера</param>
    /// <param name="location">Координата заказа на доске</param>
    public static Result<CourierAggregate, Error> Create(string name, LocationVo location)
    {
        if (string.IsNullOrWhiteSpace(name)) return GeneralErrors.ValueIsRequired(nameof(name));
        if (location is null) return GeneralErrors.ValueIsRequired(nameof(location));
        
        return new CourierAggregate(name, location);
    }

    /// <summary>
    ///     Проверяет, может ли курьер принять новый заказ
    /// </summary>
    /// <param name="order">Заказ для проверки</param>
    /// <returns>true, если заказ может быть назначен, иначе false</returns>
    public Result<bool, Error> CanAcceptOrder(OrderAggregate order)
    {
        if (order is null) return GeneralErrors.ValueIsRequired(nameof(order));

        var volumes = AssignmentsAsReadOnly.Select(a => a.Volume);
        var currentVolume = VolumeVo.Sum(volumes);

        return currentVolume + order.Volume < VolumeMax;
    }
    
    /// <summary>
    ///     Назначить заказ на курьера
    /// </summary>
    /// <param name="order">Заказ</param>
    public UnitResult<Error> AssignOrder(OrderAggregate order)
    {
        if (order is null) return GeneralErrors.ValueIsRequired(nameof(order));

        var canAcceptOrder = CanAcceptOrder(order);
        if (canAcceptOrder.IsFailure)
            return canAcceptOrder.Error;

        if (!canAcceptOrder.Value)
            return Errors.VolumeExceedsLimit();
            
        var assignment = CourierAssignmentEntity.CreateFromOrder(order);
        if (assignment.IsFailure)
            return assignment.Error;

        _assignments.Add(assignment.Value);

        return UnitResult.Success<Error>();
    }
    
    /// <summary>
    ///     Завершить заказ
    /// </summary>
    /// <param name="assignment">Назначение на курьера</param>
    public UnitResult<Error> CompleteAssignment(CourierAssignmentEntity assignment)
    {
        if (assignment is null) return GeneralErrors.ValueIsRequired(nameof(assignment));

        var result = assignment.Complete(Location);
        if (result.IsFailure)
            return result.Error;
        
        return UnitResult.Success<Error>();
    }

    /// <summary>
    ///     Переместиться в новую точку
    /// </summary>
    /// <param name="target">Назначение на курьера</param>
    public UnitResult<Error> Move(LocationVo target)
    {
        if (target is null) return GeneralErrors.ValueIsRequired(nameof(target));

        var isAdjacentTo = LocationVo.IsAdjacentTo(Location, target);
        if (isAdjacentTo.IsFailure)
            return isAdjacentTo.Error;
        
        if (!isAdjacentTo.Value)
            return  Errors.LocationNotAdjacent();
        
        return UnitResult.Success<Error>();
    }

    public static class Errors
    {
        public static Error VolumeExceedsLimit()
        {
            return new Error(
                $"{nameof(CourierAggregate).ToLowerInvariant()}.volume.exceeds.limit",
                $"Total volume exceeds maximum allowed volume");
        }

        public static Error LocationNotAdjacent()
        {
            return new Error(
                $"{nameof(CourierAggregate).ToLowerInvariant()}.location.not.adjacent",
                "Target location is not adjacent to the source location");
        }
    }
}