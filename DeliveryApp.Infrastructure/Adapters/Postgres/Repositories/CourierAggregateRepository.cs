using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class CourierAggregateRepository : BaseRepository<CourierAggregate>, ICourierAggregateRepository
{
    public CourierAggregateRepository(ApplicationDbContext context) : base(context) {}
    
    protected override IQueryable<CourierAggregate> IncludeEntities()
    {
        return Entity.Include(e => e.AssignmentsAsReadOnly);
    }
}