using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfiguration.OrderAggregateConfiguration;

public class OrderAggregateEntityTypeConfiguration : IEntityTypeConfiguration<OrderAggregate>
{
    public void Configure(EntityTypeBuilder<OrderAggregate> builder)
    {
        builder.ToTable("order");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.CourierId)
            .HasColumnName("courier_id")
            .IsRequired(false);

        builder.ComplexProperty(e => e.Location, locationBuilder =>
        {
            locationBuilder.Property(l => l.X)
                .HasColumnName("location_x")
                .IsRequired();

            locationBuilder.Property(l => l.Y)
                .HasColumnName("location_y")
                .IsRequired();
        });

        builder.Property(e => e.Volume)
            .HasConversion(
                v => v.Value,
                v => VolumeVo.Create(v).Value
            )
            .HasColumnName("volume")
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion(
                s => s.Status,
                s => OrderStatusVo.FromEnum(s)
            )
            .HasColumnName("status")
            .IsRequired();
    }
}