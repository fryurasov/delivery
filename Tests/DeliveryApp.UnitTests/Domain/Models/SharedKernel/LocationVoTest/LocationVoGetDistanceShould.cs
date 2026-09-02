using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.SharedKernel.LocationVoTest;

public class LocationVoGetDistanceShould
{
    [Theory]
    [InlineData(1, 1, 1, 1, 0)]
    [InlineData(1, 1, 5, 1, 4)]
    [InlineData(1, 1, 1, 5, 4)]
    [InlineData(1, 1, 5, 5, 8)]
    [InlineData(3, 4, 7, 9, 9)]
    [InlineData(10, 10, 1, 1, 18)]
    [InlineData(5, 5, 5, 5, 0)]
    public void BeCorrectWhenParamsAreCorrectOnCalculated(byte x1, byte y1, byte x2, byte y2, byte expectedDistance)
    {
        // Arrange
        var location1 = LocationVo.Create(x1, y1).Value;
        var location2 = LocationVo.Create(x2, y2).Value;

        // Act
        var distance = LocationVo.GetDistance(location1, location2);

        // Assert
        distance.Should().Be(expectedDistance);
    }
}
