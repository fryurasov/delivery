using System;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAssignmentEntityTest;

public class CourierAssignmentEntityCreateShould
{
    [Fact]
    public void BeSuccessWhenParametersAreValid()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(10).Value;

        // Act
        var result = CourierAssignmentEntity.Create(orderId, location, volume);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OrderId.Should().Be(orderId);
        result.Value.Location.Should().Be(location);
        result.Value.Volume.Should().Be(volume);
        result.Value.Status.Should().Be(CourierAssignmentStatusVo.Assigned);
    }

    [Fact]
    public void ReturnErrorWhenOrderIdIsEmpty()
    {
        // Arrange
        var orderId = Guid.Empty;
        var location = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(10).Value;

        // Act
        var result = CourierAssignmentEntity.Create(orderId, location, volume);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReturnErrorWhenLocationIsNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        LocationVo location = null;
        var volume = VolumeVo.Create(10).Value;

        // Act
        var result = CourierAssignmentEntity.Create(orderId, location, volume);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReturnErrorWhenVolumeIsNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        VolumeVo volume = null;

        // Act
        var result = CourierAssignmentEntity.Create(orderId, location, volume);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}