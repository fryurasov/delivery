using System;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAssignmentEntityTest;

public class CourierAssignmentCanCompleteShould
{
    [Fact]
    public void ReturnTrueWhenLocationIsEqualCourierLocation()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        var courierLocation = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(42).Value;
        var orderAssignment = CourierAssignmentEntity.Create(guid, location, volume).Value;

        // Act
        var result = orderAssignment.CanComplete(courierLocation);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(5, 5, 5, 6)]
    [InlineData(5, 5, 6, 5)]
    [InlineData(5, 6, 5, 5)]
    [InlineData(6, 5, 5, 5)]
    public void ReturnFalseWhenLocationIsNotEqualCourierLocation(
        byte orderX, byte orderY, byte courierX, byte courierY
    ) {
        // Arrange
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(orderX, orderY).Value;
        var courierLocation = LocationVo.Create(courierX, courierY).Value;
        var volume = VolumeVo.Create(42).Value;
        var orderAssignment = CourierAssignmentEntity.Create(guid, location, volume).Value;

        // Act
        var result = orderAssignment.CanComplete(courierLocation);
        
        // Assert
        result.Should().BeFalse();
    }
}
