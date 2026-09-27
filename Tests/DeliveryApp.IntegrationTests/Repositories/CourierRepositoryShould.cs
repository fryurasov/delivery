using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class CourierRepositoryShould : RepositoryTestBase
{
    [Fact]
    public async Task CanAddAndGetByIdCourier()
    {
        // Arrange
        var courier = CreateCourier(1, 1);
        
        // Act
        var courierRepository = new CourierAggregateRepository(DbContext);
        await courierRepository.AddAsync(courier);
        var unitOfWork = new UnitOfWork(DbContext);
        await unitOfWork.SaveChangesAsync();
        await unitOfWork.SaveChangesAsync();

        // Assert
        var getCourierResult = await courierRepository.GetByIdAsync(courier.Id, default);
        getCourierResult.HasValue.Should().BeTrue();
        var courierFromDb = getCourierResult.Value;
        courier.Should().BeEquivalentTo(courierFromDb);
    }
    
    [Fact]
    public async Task CanUpdateCourier()
    {
        // Arrange
        var courier = CreateCourier(1, 1);

        var courierRepository = new CourierAggregateRepository(DbContext);
        await courierRepository.AddAsync(courier);
        var unitOfWork = new UnitOfWork(DbContext);
        await unitOfWork.SaveChangesAsync();

        // Act
        var target = LocationVo.Create(1, 2).Value;
        courier.Move(target);
        courierRepository.Update(courier);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var getBasketResult = await courierRepository.GetByIdAsync(courier.Id, default);
        getBasketResult.HasValue.Should().BeTrue();
        var basketFromDb = getBasketResult.Value;
        courier.Should().BeEquivalentTo(basketFromDb);
    }

    // Хелперы для сборки объектов из примитивов
    private static CourierAggregate CreateCourier(int x, int y)
    {
        var location = LocationVo.Create((byte)x, (byte)y).Value;
        return CourierAggregate.Create("Курьер тест", location).Value;
    }
}
