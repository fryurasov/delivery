using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders.Response;
using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;

public class GetNotCompletedOrdersQuery : IRequest<GetNotCompletedOrdersResponse>
{
    public GetNotCompletedOrdersQuery ()
    {
    }

    /// <summary>
    ///     Factory Method
    /// </summary>
    public static GetNotCompletedOrdersQuery Create()
    {
        return new GetNotCompletedOrdersQuery();
    }
}