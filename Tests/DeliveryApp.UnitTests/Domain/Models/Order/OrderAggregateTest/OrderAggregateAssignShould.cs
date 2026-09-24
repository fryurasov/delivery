using System;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Order.OrderAggregateTest;

public class OrderAggregateAssignShould
{
    [Fact]
    public void BeSuccessWhenOrderIsCreatedAndCourierCanAccept()
    {
        // Arrange
        var order = CreateOrder();
        var courier = CreateCourier();

        // Act
        var result = order.Assign(courier);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatusVo.Assigned);
        order.CourierId.Should().Be(courier.Id);
    }

    [Fact]
    public void ReturnValueIsRequiredErrorWhenCourierIsNull()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        var result = order.Assign(null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired("courier"));
        order.Status.Should().Be(OrderStatusVo.Created); // Статус не должен измениться
    }

    #region Helpers

    private static OrderAggregate CreateOrder(int volumeValue = 1)
    {
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(volumeValue).Value;
        return OrderAggregate.Create(guid, location, volume).Value;
    }

    private static CourierAggregate CreateCourier()
    {
        var location = LocationVo.Create(5, 5).Value;
        return CourierAggregate.Create("Иван", location).Value;
    }

    #endregion
}