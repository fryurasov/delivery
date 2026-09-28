using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfiguration.CourierAggregateConfiguration;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfiguration.OrderAggregateConfiguration;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<CourierAggregate> Couriers { get; set; }
    public DbSet<OrderAggregate> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderAggregateEntityTypeConfiguration());
        
        modelBuilder.ApplyConfiguration(new CourierAggregateEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CourierAssignmentEntityTypeConfiguration());
    }
}
