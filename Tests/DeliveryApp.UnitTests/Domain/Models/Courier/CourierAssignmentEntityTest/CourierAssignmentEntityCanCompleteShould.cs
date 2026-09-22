using System;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAssignmentEntityTest;

public class CourierAssignmentEntityCanCompleteShould
{
    [Fact]
    public void ReturnTrueWhenCourierIsAtSameLocationAndStatusIsAssigned()
    {
        // Arrange
        var assignmentLocation = LocationVo.Create(5, 5).Value;
        var courierLocation = LocationVo.Create(5, 5).Value;
        var assignment = CreateAssignment(assignmentLocation);

        // Act
        var result = assignment.CanComplete(courierLocation);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public void ReturnFalseWhenCourierIsAtDifferentLocation()
    {
        // Arrange
        var assignmentLocation = LocationVo.Create(5, 5).Value;
        var courierLocation = LocationVo.Create(5, 6).Value; // Другая клетка
        var assignment = CreateAssignment(assignmentLocation);

        // Act
        var result = assignment.CanComplete(courierLocation);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public void ReturnAlreadyCompletedErrorWhenStatusIsCompleted()
    {
        // Arrange
        var location = LocationVo.Create(5, 5).Value;
        var assignment = CreateAssignment(location);
        assignment.Complete(location); // Переводим в статус Completed

        // Act
        var result = assignment.CanComplete(location);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourierAssignmentEntity.Errors.AlreadyCompleted());
    }

    [Fact]
    public void ReturnErrorWhenCourierLocationIsNull()
    {
        // Arrange
        var assignment = CreateAssignment(LocationVo.Create(5, 5).Value);

        // Act
        var result = assignment.CanComplete(null);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    private static CourierAssignmentEntity CreateAssignment(LocationVo location)
    {
        var orderId = Guid.NewGuid();
        var volume = VolumeVo.Create(5).Value;
        return CourierAssignmentEntity.Create(orderId, location, volume).Value;
    }
}