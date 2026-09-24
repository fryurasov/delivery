using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class OrderAggregateRepository : BaseRepository<OrderAggregate>, IOrderAggregateRepository
{
    public OrderAggregateRepository(ApplicationDbContext context) : base(context) {}

    public Task<OrderAggregate?> GetFirstCreatedAsync()
    {
        return IncludeEntities()
            .Where(o => o.Status == OrderStatusVo.Created)
            .FirstOrDefaultAsync();
    }
    
    public Task<List<OrderAggregate>> GetAssignedAsync()
    {
        return IncludeEntities()
            .Where(o => o.Status == OrderStatusVo.Assigned)
            .ToListAsync<OrderAggregate>();
    }
}
