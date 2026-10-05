using DeliveryApp.Core.Application.Queries.Common.Dto;

namespace DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;

public class CourierDto
{
    /// <summary>
    ///     Идентификатор
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    ///     Имя
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Геопозиция (X,Y)
    /// </summary>
    public required LocationDto Location { get; set; }
}