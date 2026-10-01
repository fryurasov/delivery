namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders.Response;

public class GetNotCompletedOrdersResponse
{
    public required IReadOnlyList<OrderDto> Orders { get; set; }
}