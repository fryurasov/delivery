using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Queries;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.IntegrationTests.Postgres.Queries;

public class OrderQueryServiceShould : PostgresTestBase
{
    [Fact]
    public async Task GetNotCompletedOrdersWhenNoOrdersExist()
    {
        // Arrange
        var options = CreateOptions();
        var queryService = new OrderQueryService(options);

        // Act
        var result = await queryService.GetNotCompletedOrders();

        // Assert
        result.Should().NotBeNull();
        result.Orders.Should().BeEmpty();
    }

    [Fact]
    public async Task GetNotCompletedOrdersWhenNotCompletedOrdersExist()
    {
        // Arrange
        var options = CreateOptions();

        // Создаем незавершенный (Created) и завершенный (Completed) заказы
        var createdOrder = CreateOrder(1, 1);
        var completedOrder = CreateCompletedOrder(2, 2);

        await using (var writeContext = CreateDbContext())
        {
            var orderRepository = new OrderAggregateRepository(writeContext);
            var unitOfWork = new UnitOfWork(writeContext);

            await orderRepository.AddAsync(createdOrder);
            await orderRepository.AddAsync(completedOrder);
            await unitOfWork.SaveChangesAsync();
        }

        var queryService = new OrderQueryService(options);

        // Act
        var result = await queryService.GetNotCompletedOrders();

        // Assert
        result.Should().NotBeNull();
        result.Orders.Should().HaveCount(1);
        
        var returnedOrder = result.Orders.Single();
        returnedOrder.Id.Should().Be(createdOrder.Id);
        returnedOrder.Location.X.Should().Be(createdOrder.Location.X);
        returnedOrder.Location.Y.Should().Be(createdOrder.Location.Y);
    }

    // --- Хелперы для сборки тестовых объектов ---
    private static OrderAggregate CreateOrder(int x, int y)
    {
        var location = LocationVo.Create((byte)x, (byte)y).Value;
        var volume = VolumeVo.Create(2).Value;
        var guid = Guid.NewGuid();
        return OrderAggregate.Create(guid, location, volume).Value;
    }
    
    private static CourierAggregate CreateCourier(int x, int y)
    {
        var location = LocationVo.Create((byte)x, (byte)y).Value;
        return CourierAggregate.Create("Курьер тест", location).Value;
    }

    private static OrderAggregate CreateCompletedOrder(int x, int y)
    {
        var order = CreateOrder(x, y);
        var courier = CreateCourier(x, y);
        
        order.Assign(courier); 
        order.Complete(); 
        
        return order;
    }
}