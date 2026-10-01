using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders.Response;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;

public class GetNotCompletedOrdersQueryHandler : IRequestHandler<GetNotCompletedOrdersQuery, GetNotCompletedOrdersResponse>
{
    private readonly IOrderQueryService _orderQueryService;

    public GetNotCompletedOrdersQueryHandler(IOrderQueryService orderQueryService)
    {
        _orderQueryService = orderQueryService 
                             ?? throw new ArgumentNullException(nameof(orderQueryService));
    }

    public async Task<GetNotCompletedOrdersResponse> Handle(GetNotCompletedOrdersQuery query, CancellationToken cancellationToken)
    {
        return await _orderQueryService.GetNotCompletedOrders();
    }
}
