using DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetAllCouriers;

public class GetAllCouriersQueryHandler : IRequestHandler<GetAllCouriersQuery, GetAllCouriersResponse>
{
    private readonly ICourierQueryService _courierQueryService;

    public GetAllCouriersQueryHandler(ICourierQueryService courierQueryService)
    {
        _courierQueryService = courierQueryService 
                               ?? throw new ArgumentNullException(nameof(courierQueryService));
    }

    public async Task<GetAllCouriersResponse> Handle(GetAllCouriersQuery query, CancellationToken cancellationToken)
    {
        return await _courierQueryService.GetAllCouriers();
    }
}