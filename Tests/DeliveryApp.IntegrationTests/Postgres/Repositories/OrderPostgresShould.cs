using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using DeliveryApp.IntegrationTests.Postgres;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.IntegrationTests.Postgres.Repositories;

public class OrderPostgresShould : PostgresTestBase
{
    [Fact]
    public async Task CanAddAndGetById()
    {
        // Arrange
        var order = CreateOrder(1, 1);
        
        // Act
        await using (var writeContext = CreateDbContext())
        {
            var orderRepository = new OrderAggregateRepository(writeContext);
            var unitOfWork = new UnitOfWork(writeContext);
            
            await orderRepository.AddAsync(order);
            await unitOfWork.SaveChangesAsync();
        }

        // Assert
        await using (var readContext = CreateDbContext())
        {
            var orderRepository = new OrderAggregateRepository(readContext);
            
            var getOrderResult = await orderRepository.GetByIdAsync(order.Id, default);
            getOrderResult.HasValue.Should().BeTrue();
            
            var orderFromDb = getOrderResult.Value;
            order.Should().BeEquivalentTo(orderFromDb);
        }
    }
    
    [Fact]
    public async Task CanUpdate()
    {
        // Arrange
        var order = CreateOrder(1, 1);

        await using (var writeContext = CreateDbContext())
        {
            var orderRepository = new OrderAggregateRepository(writeContext);
            var unitOfWork = new UnitOfWork(writeContext);
            
            await orderRepository.AddAsync(order);
            await unitOfWork.SaveChangesAsync();
        }

        // Act
        OrderAggregate orderToUpdate;
        
        await using (var updateContext = CreateDbContext())
        {
            var orderRepository = new OrderAggregateRepository(updateContext);
            var unitOfWork = new UnitOfWork(updateContext);
            
            orderToUpdate = (await orderRepository.GetByIdAsync(order.Id, default)).Value;
            
            var location = LocationVo.Create((byte)1, (byte)1).Value;
            var courier = CourierAggregate.Create("Тестовый курьер", location).Value;
            orderToUpdate.Assign(courier);
            
            orderRepository.Update(orderToUpdate);
            await unitOfWork.SaveChangesAsync();
        }
        
        // Assert
        await using (var readContext = CreateDbContext())
        {
            var orderRepository = new OrderAggregateRepository(readContext);
            
            var getOrderResult = await orderRepository.GetByIdAsync(order.Id, default);
            getOrderResult.HasValue.Should().BeTrue();

            var orderFromDb = getOrderResult.Value;
            orderToUpdate.Should().BeEquivalentTo(orderFromDb);
        }
    }

    [Fact]
    public async Task CanGetAll()
    {
        // Arrange
        var order1 = CreateOrder(1, 1);
        var order2 = CreateOrder(2, 2);
        var order3 = CreateOrder(3, 3);

        var expectedOrders = new List<OrderAggregate> { order1, order2, order3 };
        
        await using (var writeContext = CreateDbContext())
        {
            var orderRepository = new OrderAggregateRepository(writeContext);
            var unitOfWork = new UnitOfWork(writeContext);
            
            await orderRepository.AddAsync(order1);
            await orderRepository.AddAsync(order2);
            await orderRepository.AddAsync(order3);
            await unitOfWork.SaveChangesAsync();
        }

        // Act

        // Assert
        await using (var readContext = CreateDbContext())
        {
            var orderRepository = new OrderAggregateRepository(readContext);
            var ordersFromDb = await orderRepository.GetAllAsync(default);

            ordersFromDb.Should().HaveCount(3);
            ordersFromDb.Should().BeEquivalentTo(expectedOrders);
        }
    }
    
    // Хелперы для сборки объектов из примитивов
    private static OrderAggregate CreateOrder(int x, int y)
    {
        var location = LocationVo.Create((byte)x, (byte)y).Value;
        var volume = VolumeVo.Create(4).Value;
        var basketId = Guid.NewGuid();
        
        return OrderAggregate.Create(basketId, location, volume).Value;
    }
}