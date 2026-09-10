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
    ///     Проверяет, что переход из старого статуса в новый допустим
    /// </summary>
    /// <param name="oldStatus">Старый статус</param>
    /// <param name="newStatus">Новый статус</param>
    public static UnitResult<Error> EnsureCanTransitionTo(OrderStatusVo oldStatus, OrderStatusVo newStatus)
    {
        if (newStatus == oldStatus)
            return Errors.AlreadyInStatus(oldStatus);
        
        if (newStatus.Status == OrderStatusEnum.Assigned && oldStatus.Status != OrderStatusEnum.Created)
            return Errors.OldStatusNotEqualCreated(oldStatus);
        
        if (newStatus.Status == OrderStatusEnum.Completed && oldStatus.Status != OrderStatusEnum.Assigned)
            return Errors.OldStatusNotEqualAssigned(oldStatus);

        return UnitResult.Success<Error>();
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
    }
}
