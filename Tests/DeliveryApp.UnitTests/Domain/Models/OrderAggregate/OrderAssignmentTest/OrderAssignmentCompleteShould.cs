using System;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.OrderAggregate.OrderAssignmentTest;

public class OrderAssignmentCompleteShould
{
    [Fact]
    public void ReturnTrueWhenLocationIsEqualCourierLocation()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        var courierLocation = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(42).Value;
        var orderAssignment = OrderAssignmentEntity.Create(guid, location, volume).Value;

        // Act
        var result = orderAssignment.CanComplete(courierLocation);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ReturnFalseWhenLocationIsNotEqualCourierLocation()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        var courierLocation = LocationVo.Create(6, 5).Value;
        var volume = VolumeVo.Create(42).Value;
        var orderAssignment = OrderAssignmentEntity.Create(guid, location, volume).Value;

        // Act
        var result = orderAssignment.CanComplete(courierLocation);
        
        // Assert
        result.Should().BeFalse();
    }
}
