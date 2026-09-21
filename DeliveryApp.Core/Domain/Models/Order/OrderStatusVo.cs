using CSharpFunctionalExtensions;
using Errs;

namespace DeliveryApp.Core.Domain.Models.Order;

/// <summary>
///     Статус назначеного заказа
/// </summary>
public sealed class OrderStatusVo : ValueObject
{
    public static OrderStatusVo Created => new(OrderStatusEnum.Created);
    public static OrderStatusVo Assigned => new(OrderStatusEnum.Assigned);
    public static OrderStatusVo Completed => new(OrderStatusEnum.Completed);
    
    public OrderStatusEnum Status { get; private set; }
    
    private OrderStatusVo(OrderStatusEnum status)
    {
        Status = status;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Status;
    }

    /// <summary>
    ///     Возвращает true, если переход из старого статуса в новый допустим, иначе false
    /// </summary>
    /// <param name="newStatus">Новый статус</param>
    public bool CanTransitionTo(OrderStatusVo newStatus)
    {
        return EnsureCanTransitionTo(newStatus).IsSuccess;
    }
    
    /// <summary>
    ///     Проверяет, что переход из старого статуса в новый допустим
    /// </summary>
    /// <param name="newStatus">Новый статус</param>
    public UnitResult<Error> EnsureCanTransitionTo(OrderStatusVo newStatus)
    {
        if (this == newStatus)
            return Errors.AlreadyInStatus(this);

        return (Status, newStatus.Status) switch
        {
            (OrderStatusEnum.Created, OrderStatusEnum.Assigned) => UnitResult.Success<Error>(),
            (OrderStatusEnum.Assigned, OrderStatusEnum.Completed) => UnitResult.Success<Error>(),
            
            (_, OrderStatusEnum.Assigned) => Errors.OldStatusNotEqualCreated(this),
            (_, OrderStatusEnum.Completed) => Errors.OldStatusNotEqualAssigned(this),
            _ => Errors.InvalidTransition(this, newStatus)
        };
    }
    
    public enum OrderStatusEnum
    {
        Created,
        Assigned,
        Completed
    }

    public static class Errors
    {
        public static Error OldStatusNotEqualCreated(OrderStatusVo currentStatus)
        {
            return new Error(
                $"{nameof(OrderStatusVo).ToLowerInvariant()}.old.status.not.equal.created",
                $"Current status is '{currentStatus.Status}', but must be '{OrderStatusEnum.Created}'");
        }
        
        public static Error OldStatusNotEqualAssigned(OrderStatusVo currentStatus)
        {
            return new Error(
                $"{nameof(OrderStatusVo).ToLowerInvariant()}.old.status.not.equal.assigned",
                $"Current status is '{currentStatus.Status}', but must be '{OrderStatusEnum.Assigned}'");
        }
        
        public static Error AlreadyInStatus(OrderStatusVo currentStatus)
        {
            return new Error(
                $"{nameof(OrderStatusVo).ToLowerInvariant()}.old.status.already.in.new.status",
                $"Already in '{currentStatus.Status}' status");
        }
        
        public static Error InvalidTransition(OrderStatusVo currentStatus, OrderStatusVo newStatus)
        {
            return new Error(
                $"{nameof(OrderStatusVo).ToLowerInvariant()}.invalid.transition",
                $"Cannot transition order status from '{currentStatus.Status}' to '{newStatus.Status}'");
        }
    }
}
