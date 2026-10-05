using DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;

namespace DeliveryApp.Core.Ports;

public interface ICourierQueryService
{
    public Task<GetAllCouriersResponse> GetAllCouriers();
}