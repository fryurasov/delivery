using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Models.Courier;

/// <summary>
///     Статус назначеного заказа
/// </summary>
public sealed class CourierAssignmentStatusVo : ValueObject
{
    public static CourierAssignmentStatusVo Assigned => new(AssignmentStatusEnum.Assigned);
    public static CourierAssignmentStatusVo Completed => new(AssignmentStatusEnum.Completed);
    
    public AssignmentStatusEnum Status { get; private set; }
    
    private CourierAssignmentStatusVo(AssignmentStatusEnum status)
    {
        Status = status;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Status;
    }

    public enum AssignmentStatusEnum
    {
        Assigned,
        Completed
    }
}
