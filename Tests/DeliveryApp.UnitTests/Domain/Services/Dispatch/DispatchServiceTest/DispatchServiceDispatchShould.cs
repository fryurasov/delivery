using System;
using System.Collections.Generic;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Domain.Services.Dispatch;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services.Dispatch.DispatchServiceTest;

public class DispatchServiceDispatchShould
{
    private readonly DispatchService _sut = new();

    [Fact]
    public void SuccessfullyAssignNearestCourierWhenOrderIsCreatedAndCouriersAreAvailable()
    {
        // Arrange
        var targetLocation = CreateLocation(1, 1);
        var order = CreateOrder(targetLocation, volumeValue: 1);

        // Ближний курьер (координата 2,2)
        var nearCourier = CreateCourier("Near Courier", CreateLocation(2, 2));
        
        // Дальний курьер (координата 10,10)
        var farCourier = CreateCourier("Far Courier", CreateLocation(10, 10));

        var couriers = new[] { farCourier, nearCourier };

        // Act
        var result = _sut.Dispatch(order, couriers);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(nearCourier);

        // Проверяем изменение состояния доменных объектов
        order.Status.Should().Be(OrderStatusVo.Assigned);
        nearCourier.AssignmentsAsReadOnly.Should().ContainSingle(a => a.OrderId == order.Id);
    }

    [Theory]
    [MemberData(nameof(GetInvalidOrderStatuses))]
    public void ReturnOrderIsNotCreatedErrorWhenOrderStatusIsNotCreated(OrderStatusVo targetStatus)
    {
        // Arrange
        var order = CreateOrder(CreateLocation(1, 1));
        
        // Переводим статус в невалидный
        if (targetStatus == OrderStatusVo.Assigned)
        {
            var courier = CreateCourier("Temp Courier", CreateLocation(1, 1));
            order.Assign(courier);
        }
        else if (targetStatus == OrderStatusVo.Completed)
        {
            var courier = CreateCourier("Temp Courier", CreateLocation(1, 1));
            order.Assign(courier);
            order.Complete();
        }

        var couriers = new[] { CreateCourier("Courier", CreateLocation(2, 2)) };

        // Act
        var result = _sut.Dispatch(order, couriers);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DispatchService.Errors.OrderIsNotCreated());
    }

    [Fact]
    public void ReturnCourierNotFoundErrorWhenCourierListIsEmpty()
    {
        // Arrange
        var order = CreateOrder(CreateLocation(1, 1));
        var couriers = new List<CourierAggregate>();

        // Act
        var result = _sut.Dispatch(order, couriers);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DispatchService.Errors.CourierNotFound());
    }

    [Fact]
    public void ReturnCourierNotFoundErrorWhenCourierExceedsMaxVolumeLimit()
    {
        // Arrange
        // Создаем заказ, объем которого превышает VolumeMax курьера
        var largeVolumeOrder = CreateOrder(CreateLocation(1, 1), volumeValue: VolumeVo.CourierVolumeMax.Value + 1);
        var couriers = new[] { CreateCourier("Courier", CreateLocation(2, 2)) };

        // Act
        var result = _sut.Dispatch(largeVolumeOrder, couriers);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DispatchService.Errors.CourierNotFound());
    }

    // В дата-провайдерах только Enum/Value Objects
    public static TheoryData<OrderStatusVo> GetInvalidOrderStatuses() => new()
    {
        OrderStatusVo.Assigned,
        OrderStatusVo.Completed
    };

    #region Helpers (Фабрики для тестов)

    private static LocationVo CreateLocation(byte x, byte y)
    {
        // Замени на реальное создание твоей LocationVo
        var locationResult = LocationVo.Create(x, y);
        return locationResult.Value;
    }

    private static OrderAggregate CreateOrder(LocationVo location, int volumeValue = 1)
    {
        var volume = VolumeVo.Create(volumeValue).Value;
        var orderResult = OrderAggregate.Create(Guid.NewGuid(), location, volume);
        return orderResult.Value;
    }

    private static CourierAggregate CreateCourier(string name, LocationVo location)
    {
        var courierResult = CourierAggregate.Create(name, location);
        return courierResult.Value;
    }

    #endregion
}