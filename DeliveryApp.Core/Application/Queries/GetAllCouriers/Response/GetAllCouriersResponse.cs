namespace DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;

public class GetAllCouriersResponse
{
    public required IReadOnlyList<CourierDto> Couriers { get; set; }
}