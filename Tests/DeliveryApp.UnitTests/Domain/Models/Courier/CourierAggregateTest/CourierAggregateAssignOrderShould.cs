using System;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAggregateTest;

public class CourierAggregateAssignOrderShould
{
    [Fact]
    public void BeSuccessWhenOrderIsValidAndVolumeIsWithinLimit()
    {
        // Arrange
        var courier = CreateCourier();
        var order = CreateOrder(volumeValue: 10);

        // Act
        var result = courier.AssignOrder(order);

        // Assert
        result.IsSuccess.Should().BeTrue();
        courier.AssignmentsAsReadOnly.Should().HaveCount(1);
    }

    [Fact]
    public void ReturnOrderAlreadyAssignedErrorWhenAssigningSameOrderTwice()
    {
        // Arrange
        var courier = CreateCourier();
        var order = CreateOrder(volumeValue: 5);
        courier.AssignOrder(order);

        // Act
        var result = courier.AssignOrder(order);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourierAggregate.Errors.OrderAlreadyAssigned());
    }

    [Fact]
    public void ReturnVolumeExceedsLimitErrorWhenOrderIsTooLarge()
    {
        // Arrange
        var courier = CreateCourier();
        var order1 = CreateOrder(volumeValue: 15);
        var order2 = CreateOrder(volumeValue: 10); // 15 + 10 = 25 > 20

        courier.AssignOrder(order1);

        // Act
        var result = courier.AssignOrder(order2);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourierAggregate.Errors.VolumeExceedsLimit());
    }

    private static CourierAggregate CreateCourier()
    {
        var location = LocationVo.Create(1, 1).Value;
        return CourierAggregate.Create("Иван", location).Value;
    }

    private static OrderAggregate CreateOrder(int volumeValue)
    {
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(1, 1).Value;
        var volume = VolumeVo.Create(volumeValue).Value;
        return OrderAggregate.Create(guid, location, volume).Value;
    }
}