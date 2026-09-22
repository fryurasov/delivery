using System;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Order.OrderAggregateTest;

public class OrderAggregateCreateShould
{
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

    [Fact]
    public void ReturnErrorWhenBasketIdIsEmpty()
    {
        // Arrange
        var basketId = Guid.Empty;
        var location = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(42).Value;

        // Act
        var result = OrderAggregate.Create(basketId, location, volume);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired(nameof(basketId)));
    }

    [Fact]
    public void ReturnErrorWhenLocationIsNull()
    {
        // Arrange
        var guid = Guid.NewGuid();
        LocationVo location = null!;
        var volume = VolumeVo.Create(42).Value;

        // Act
        var result = OrderAggregate.Create(guid, location, volume);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired(nameof(location)));
    }

    [Fact]
    public void ReturnErrorWhenVolumeIsNull()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        VolumeVo volume = null!;

        // Act
        var result = OrderAggregate.Create(guid, location, volume);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired(nameof(volume)));
    }
}