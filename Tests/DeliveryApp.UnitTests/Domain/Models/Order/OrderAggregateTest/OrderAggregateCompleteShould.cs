using System;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Order.OrderAggregateTest;

public class OrderAggregateCompleteShould
{
    [Fact]
    public void BeSuccessWhenStatusIsAssigned()
    {
        // Arrange
        var order = CreateOrder();
        order.Assign(); // Заказ должен быть назначен перед завершением

        // Act
        var result = order.Complete();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatusVo.Completed);
    }

    [Fact]
    public void ReturnErrorWhenOrderIsCreated()
    {
        // Arrange
        var order = CreateOrder(); // Статус заказа — Created

        // Act
        var result = order.Complete(); // Пытаемся завершить еще не назначенный заказ

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderStatusVo.Errors.OldStatusNotEqualAssigned(OrderStatusVo.Created));
    }

    [Fact]
    public void ReturnAlreadyInStatusErrorWhenAlreadyCompleted()
    {
        // Arrange
        var order = CreateOrder();
        order.Assign();
        order.Complete(); // Заказ уже переведен в Completed

        // Act
        var result = order.Complete(); // Пытаемся завершить повторно

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderStatusVo.Errors.AlreadyInStatus(OrderStatusVo.Completed));
    }

    private static OrderAggregate CreateOrder()
    {
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(42).Value;
        return OrderAggregate.Create(guid, location, volume).Value;
    }
}