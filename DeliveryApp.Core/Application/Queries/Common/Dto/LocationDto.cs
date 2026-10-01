namespace DeliveryApp.Core.Application.Queries.Common.Dto;

public class LocationDto
{
    /// <summary>
    ///     Горизонталь
    /// </summary>
    public required int X { get; set; }

    /// <summary>
    ///     Вертикаль
    /// </summary>
    public required int Y { get; set; }
}