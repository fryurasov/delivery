using System;
using System.Linq;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAggregateTest;

public class CourierAggregateCompleteAssignmentShould
{
    [Fact]
    public void BeSuccessWhenAssignmentBelongsToCourierAndCourierIsAtOrderLocation()
    {
        // Arrange
        var courierLocation = LocationVo.Create(1, 1).Value;
        var courier = CourierAggregate.Create("Иван", courierLocation).Value;

        // Заказ находится в той же клетке (1,1)
        var order = CreateOrderAt(courierLocation);
        courier.AssignOrder(order);

        var assignment = courier.AssignmentsAsReadOnly.First();

        // Act
        var result = courier.CompleteAssignment(assignment);

        // Assert
        result.IsSuccess.Should().BeTrue();
        assignment.Status.Should().Be(CourierAssignmentStatusVo.Completed);
    }

    [Fact]
    public void ReturnAssignmentNotFoundErrorWhenAssignmentDoesNotBelongToCourier()
    {
        // Arrange
        var courier = CourierAggregate.Create("Иван", LocationVo.Create(1, 1).Value).Value;

        // Создаем "чужое" назначение через фабричный метод
        var foreignOrder = CreateOrderAt(LocationVo.Create(1, 1).Value);
        var foreignAssignment = CourierAssignmentEntity.CreateFromOrder(foreignOrder).Value;

        // Act
        var result = courier.CompleteAssignment(foreignAssignment);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourierAggregate.Errors.AssignmentNotFound());
    }

    private static OrderAggregate CreateOrderAt(LocationVo location)
    {
        var guid = Guid.NewGuid();
        var volume = VolumeVo.Create(5).Value;
        return OrderAggregate.Create(guid, location, volume).Value;
    }
}