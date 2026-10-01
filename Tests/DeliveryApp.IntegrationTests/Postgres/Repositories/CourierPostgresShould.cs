using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using DeliveryApp.IntegrationTests.Postgres;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.IntegrationTests.Postgres.Repositories;

public class CourierPostgresShould : PostgresTestBase
{
    [Fact]
    public async Task CanAddAndGetById()
    {
        // Arrange
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
        await using (var readContext = CreateDbContext())
        {
            var courierRepository = new CourierAggregateRepository(readContext);
            
            var getCourierResult = await courierRepository.GetByIdAsync(courier.Id, default);
            getCourierResult.HasValue.Should().BeTrue();
            
            var courierFromDb = getCourierResult.Value;
            courier.Should().BeEquivalentTo(courierFromDb);
        }
    }
    
    [Fact]
    public async Task CanUpdate()
    {
        // Arrange
        var courier = CreateCourier(1, 1);

        await using (var writeContext = CreateDbContext())
        {
            var courierRepository = new CourierAggregateRepository(writeContext);
            var unitOfWork = new UnitOfWork(writeContext);
            
            await courierRepository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();
        }

        // Act
        CourierAggregate courierToUpdate;
        
        await using (var updateContext = CreateDbContext())
        {
            var courierRepository = new CourierAggregateRepository(updateContext);
            var unitOfWork = new UnitOfWork(updateContext);
            
            courierToUpdate = (await courierRepository.GetByIdAsync(courier.Id, default)).Value;
            
            var target = LocationVo.Create(1, 2).Value;
            courierToUpdate.Move(target);
            
            courierRepository.Update(courierToUpdate);
            await unitOfWork.SaveChangesAsync();
        }

        // Assert
        await using (var readContext = CreateDbContext())
        {
            var courierRepository = new CourierAggregateRepository(readContext);
            
            var getCourierResult = await courierRepository.GetByIdAsync(courier.Id, default);
            getCourierResult.HasValue.Should().BeTrue();

            var courierFromDb = getCourierResult.Value;
            courierToUpdate.Should().BeEquivalentTo(courierFromDb);
        }
    }

    [Fact]
    public async Task CanGetAll()
    {
        // Arrange
        var courier1 = CreateCourier(1, 1);
        var courier2 = CreateCourier(2, 2);
        var courier3 = CreateCourier(3, 3);

        var expectedCouriers = new List<CourierAggregate> { courier1, courier2, courier3 };
        
        await using (var writeContext = CreateDbContext())
        {
            var courierRepository = new CourierAggregateRepository(writeContext);
            var unitOfWork = new UnitOfWork(writeContext);
            
            await courierRepository.AddAsync(courier1);
            await courierRepository.AddAsync(courier2);
            await courierRepository.AddAsync(courier3);
            await unitOfWork.SaveChangesAsync();
        }

        // Act

        // Assert
        await using (var readContext = CreateDbContext())
        {
            var courierRepository = new CourierAggregateRepository(readContext);
            var couriersFromDb = await courierRepository.GetAllAsync(default);

            couriersFromDb.Should().HaveCount(3);
            couriersFromDb.Should().BeEquivalentTo(expectedCouriers);
        }
    }
    
    // Хелперы для сборки объектов из примитивов
    private static CourierAggregate CreateCourier(int x, int y)
    {
        var location = LocationVo.Create((byte)x, (byte)y).Value;
        return CourierAggregate.Create("Курьер тест", location).Value;
    }
}
