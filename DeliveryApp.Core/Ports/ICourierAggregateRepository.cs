using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Models.Courier;

namespace DeliveryApp.Core.Ports;

public interface ICourierAggregateRepository : IRepository<CourierAggregate>
{
    Task AddAsync(CourierAggregate courier);
    void Update(CourierAggregate courier);
    Task<Maybe<CourierAggregate>> GetByIdAsync(Guid courierId, CancellationToken cancellationToken);
    Task<List<CourierAggregate>> GetAllAsync(CancellationToken cancellationToken);
    public Task<bool> IsExistByIdAsync(Guid id, CancellationToken cancellationToken);
}