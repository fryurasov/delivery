using System;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAggregateTest;

public class CourierAggregateCanAcceptOrderShould
{
    [Fact]
    public void ReturnTrueWhenTotalVolumeIsWithinLimit()
    {
        // Arrange
        var courier = CreateCourier();
        var order = CreateOrder(volumeValue: 10); // Объем 10 <= VolumeMax (20)

        // Act
        var result = courier.CanAcceptOrder(order);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public void ReturnTrueWhenTotalVolumeIsExactlyEqualToLimit()
    {
        // Arrange
        var courier = CreateCourier();
        var order = CreateOrder(volumeValue: 20); // Объем ровно 20 (граница)

        // Act
        var result = courier.CanAcceptOrder(order);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public void ReturnFalseWhenTotalVolumeExceedsLimit()
    {
        // Arrange
        var courier = CreateCourier();
        var order = CreateOrder(volumeValue: 21); // 21 > VolumeMax (20)

        // Act
        var result = courier.CanAcceptOrder(order);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
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