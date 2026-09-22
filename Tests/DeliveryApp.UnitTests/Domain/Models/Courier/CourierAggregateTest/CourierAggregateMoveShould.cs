using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAggregateTest;

public class CourierAggregateMoveShould
{
    [Fact]
    public void BeSuccessAndChangeLocationWhenTargetIsAdjacent()
    {
        // Arrange
        var startLocation = LocationVo.Create(1, 1).Value;
        var targetLocation = LocationVo.Create(1, 2).Value; // Смежная клетка (1 шаг)
        var courier = CourierAggregate.Create("Иван", startLocation).Value;

        // Act
        var result = courier.Move(targetLocation);

        // Assert
        result.IsSuccess.Should().BeTrue();
        courier.Location.Should().Be(targetLocation);
    }

    [Fact]
    public void ReturnLocationNotAdjacentErrorWhenTargetIsTooFar()
    {
        // Arrange
        var startLocation = LocationVo.Create(1, 1).Value;
        var farLocation = LocationVo.Create(5, 5).Value; // Слишком далеко
        var courier = CourierAggregate.Create("Иван", startLocation).Value;

        // Act
        var result = courier.Move(farLocation);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourierAggregate.Errors.LocationNotAdjacent());
        courier.Location.Should().Be(startLocation); // Позиция не изменилась
    }
}