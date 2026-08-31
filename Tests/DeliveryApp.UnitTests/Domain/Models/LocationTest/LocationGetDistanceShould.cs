using DeliveryApp.Core.Domain.Models;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.LocationTest;

public class LocationGetDistanceShould
{
    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(0, 0, 5, 0, 5)]
    [InlineData(0, 0, 0, 5, 5)]
    [InlineData(0, 0, 5, 5, 10)]
    [InlineData(3, 4, 7, 9, 9)]
    [InlineData(10, 10, 0, 0, 20)]
    [InlineData(5, 5, 5, 5, 0)]
    public void BeCorrectWhenParamsAreCorrectOnCalculated(byte x1, byte y1, byte x2, byte y2, byte expectedDistance)
    {
        // Arrange
        var location1 = Location.Create(x1, y1).Value;
        var location2 = Location.Create(x2, y2).Value;

        // Act
        var distance = Location.GetDistance(location1, location2);

        // Assert
        distance.Should().Be(expectedDistance);
    }
}