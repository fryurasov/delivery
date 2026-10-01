using DeliveryApp.Core.Application.Queries.Common.Dto;

namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders.Response;

public class OrderDto
{
    /// <summary>
    ///     Идентификатор
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    ///     Геопозиция (X,Y)
    /// </summary>
    public required LocationDto Location { get; set; }
}