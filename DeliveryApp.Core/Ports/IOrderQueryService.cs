using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders.Response;

namespace DeliveryApp.Core.Ports;

public interface IOrderQueryService
{
    public Task<GetNotCompletedOrdersResponse> GetNotCompletedOrders();
}