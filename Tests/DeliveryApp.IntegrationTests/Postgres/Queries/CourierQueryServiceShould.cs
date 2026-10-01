using DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Queries;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using DeliveryApp.IntegrationTests.Postgres;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.IntegrationTests.Postgres.Queries;

public class CourierQueryServiceShould : PostgresTestBase
{
    [Fact]
    public async Task ReturnEmptyListWhenNoCouriersExist()
    {
        // Arrange
        var readContext = CreateOptions();
        
        // Act

        // Assert
        var queryService = new CourierQueryService(readContext);
        var getAllCouriers = await queryService.GetAllCouriers();
        
        getAllCouriers.Should().NotBeNull();
        getAllCouriers.Couriers.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetAllCouriersWhenCouriersExist()
    {
        // Arrange
        var readContext = CreateOptions();
        var courier = CreateCourier(1, 1);

        // Act
        await using (var writeContext = CreateDbContext())
        {
            var courierRepository = new CourierAggregateRepository(writeContext);
            var unitOfWork = new UnitOfWork(writeContext);
            
            await courierRepository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();
        }

        // Assert
        var queryService = new CourierQueryService(readContext);
        var getAllCouriers = await queryService.GetAllCouriers();
        
        getAllCouriers.Should().NotBeNull();
        getAllCouriers.Couriers.Should().NotBeEmpty();
    }
    
    // Хелперы для сборки объектов из примитивов
    private static CourierAggregate CreateCourier(int x, int y)
    {
        var location = LocationVo.Create((byte)x, (byte)y).Value;
        return CourierAggregate.Create("Курьер тест", location).Value;
    }
}