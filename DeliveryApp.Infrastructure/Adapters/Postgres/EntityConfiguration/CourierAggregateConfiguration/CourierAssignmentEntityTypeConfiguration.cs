using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfiguration.CourierAggregateConfiguration;

public class CourierAssignmentEntityTypeConfiguration : IEntityTypeConfiguration<CourierAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<CourierAssignmentEntity> builder)
    {
        builder.ToTable("courier_assignment");

        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.OrderId)
            .HasColumnName("order_id")
            .IsRequired();
        
        builder.Property<Guid>("CourierId")
            .HasColumnName("courier_id")
            .IsRequired();

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
                s => CourierAssignmentStatusVo.FromEnum(s)
            )
            .HasColumnName("status")
            .IsRequired();
    }
}