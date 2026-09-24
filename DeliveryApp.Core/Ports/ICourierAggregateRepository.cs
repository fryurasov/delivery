using Ddd;
using DeliveryApp.Core.Domain.Models.Courier;

namespace DeliveryApp.Core.Ports;

public interface ICourierAggregateRepository : IRepository<CourierAggregate>
{
    Task AddAsync(CourierAggregate courier);
    void Update(CourierAggregate courier);
    Task<CourierAggregate?> GetByIdAsync(Guid courierId);
    Task<List<CourierAggregate>> GetAllAsync();
}