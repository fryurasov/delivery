using DeliveryApp.Core.Domain.Models.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfiguration.CourierAggregateConfiguration;

internal class CourierAggregateEntityTypeConfiguration : IEntityTypeConfiguration<CourierAggregate>
{
    public void Configure(EntityTypeBuilder<CourierAggregate> entityTypeBuilder)
    {
        entityTypeBuilder.ToTable("courier");

        entityTypeBuilder.HasKey(entity => entity.Id);

        entityTypeBuilder
            .Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .IsRequired();

        entityTypeBuilder
            .Property(entity => entity.Name)
            .HasColumnName("name")
            .IsRequired();

        entityTypeBuilder
            .ComplexProperty(entity => entity.Location, locationBuilder =>
            {
                locationBuilder.Property(l => l.X)
                    .HasColumnName("x")
                    .IsRequired();
                
                locationBuilder.Property(l => l.Y)
                    .HasColumnName("y")
                    .IsRequired();
            });
        
        entityTypeBuilder.HasMany(c => c.Assignments)
            .WithOne()
            .HasForeignKey("CourierId")
            .OnDelete(DeleteBehavior.Cascade);

        entityTypeBuilder.Navigation(c => c.Assignments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
