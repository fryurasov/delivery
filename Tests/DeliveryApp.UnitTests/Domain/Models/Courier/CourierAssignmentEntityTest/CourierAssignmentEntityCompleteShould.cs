using System;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAssignmentEntityTest;

public class CourierAssignmentEntityCompleteShould
{
    [Fact]
    public void BeSuccessAndChangeStatusToCompletedWhenCourierIsAtOrderLocation()
    {
        // Arrange
        var location = LocationVo.Create(5, 5).Value;
        var assignment = CreateAssignment(location);

        // Act
        var result = assignment.Complete(location);

        // Assert
        result.IsSuccess.Should().BeTrue();
        assignment.Status.Should().Be(CourierAssignmentStatusVo.Completed);
    }

    [Fact]
    public void ReturnCourierTooFarErrorWhenCourierIsAtDifferentLocation()
    {
        // Arrange
        var assignmentLocation = LocationVo.Create(5, 5).Value;
        var courierLocation = LocationVo.Create(5, 6).Value;
        var assignment = CreateAssignment(assignmentLocation);

        // Act
        var result = assignment.Complete(courierLocation);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourierAssignmentEntity.Errors.CourierTooFar());
        assignment.Status.Should().Be(CourierAssignmentStatusVo.Assigned); // Статус не изменился
    }

    [Fact]
    public void ReturnAlreadyCompletedErrorWhenCompletingAlreadyCompletedAssignment()
    {
        // Arrange
        var location = LocationVo.Create(5, 5).Value;
        var assignment = CreateAssignment(location);
        assignment.Complete(location); // Первое успешное завершение

        // Act
        var result = assignment.Complete(location); // Вторая попытка

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
        var result = assignment.Complete(null);

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