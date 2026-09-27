using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class OrderAggregateRepository : BaseRepository<OrderAggregate>, IOrderAggregateRepository
{
    public OrderAggregateRepository(ApplicationDbContext context) : base(context) {}

    public Task<Maybe<OrderAggregate>> GetFirstCreatedAsync(CancellationToken cancellationToken)
    {
        return IncludeEntities()
            .Where(o => o.Status == OrderStatusVo.Created)
            .FirstOrDefaultAsync(cancellationToken)
            .AsMaybe();
    }
    
    public Task<List<OrderAggregate>> GetAssignedAsync(CancellationToken cancellationToken)
    {
        return IncludeEntities()
            .Where(o => o.Status == OrderStatusVo.Assigned)
            .ToListAsync(cancellationToken);
    }
}
