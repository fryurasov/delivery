using System;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAssignmentEntityTest;

public class CourierAssignmentEntityCreateFromOrderShould
{
    [Fact]
    public void BeSuccessWhenOrderIsValid()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var location = LocationVo.Create(3, 3).Value;
        var volume = VolumeVo.Create(7).Value;
        var order = OrderAggregate.Create(orderId, location, volume).Value;

        // Act
        var result = CourierAssignmentEntity.CreateFromOrder(order);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OrderId.Should().Be(orderId);
        result.Value.Location.Should().Be(location);
        result.Value.Volume.Should().Be(volume);
        result.Value.Status.Should().Be(CourierAssignmentStatusVo.Assigned);
    }

    [Fact]
    public void ReturnErrorWhenOrderIsNull()
    {
        // Arrange
        OrderAggregate order = null;

        // Act
        var result = CourierAssignmentEntity.CreateFromOrder(order);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}