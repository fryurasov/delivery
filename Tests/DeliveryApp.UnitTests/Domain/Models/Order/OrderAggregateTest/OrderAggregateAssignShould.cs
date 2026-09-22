using System;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Order.OrderAggregateTest;

public class OrderAggregateAssignShould
{
    [Fact]
    public void BeSuccessWhenStatusIsCreated()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        var result = order.Assign();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatusVo.Assigned);
    }

    [Fact]
    public void ReturnAlreadyInStatusErrorWhenAlreadyAssigned()
    {
        // Arrange
        var order = CreateOrder();
        order.Assign(); // Назначаем в первый раз, статус становится Assigned

        // Act
        var result = order.Assign(); // Пытаемся назначить повторно

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderStatusVo.Errors.AlreadyInStatus(OrderStatusVo.Assigned));
    }

    [Fact]
    public void ReturnErrorWhenOrderIsCompleted()
    {
        // Arrange
        var order = CreateOrder();
        order.Assign();
        order.Complete(); // Переводим в статус Completed

        // Act
        var result = order.Assign(); // Пытаемся назначить завершенный заказ

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderStatusVo.Errors.OldStatusNotEqualCreated(OrderStatusVo.Completed));
    }

    private static OrderAggregate CreateOrder()
    {
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(42).Value;
        return OrderAggregate.Create(guid, location, volume).Value;
    }
}