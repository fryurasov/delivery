using DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;
using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetAllCouriers;

public class GetAllCouriersQuery : IRequest<GetAllCouriersResponse>
{
    public GetAllCouriersQuery ()
    {
    }

    /// <summary>
    ///     Factory Method
    /// </summary>
    public static GetAllCouriersQuery Create()
    {
        return new GetAllCouriersQuery();
    }
}