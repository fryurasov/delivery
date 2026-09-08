using System;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Order.OrderAggregateTest;

public class OrderAggregateCreateShould
{
    // Добавить тест когда guid пуст
    // Проверить получу ли ошибку, если передам null в location или volume
    [Fact]
    public void BeCorrectWhenParamsAreCorrectOnCreated()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(42).Value;

        // Act
        var result = OrderAggregate.Create(guid, location, volume);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(OrderStatusVo.Created);
        result.Value.Id.Should().Be(guid);
        result.Value.Location.Should().Be(location);
        result.Value.Volume.Should().Be(volume);
    }
}