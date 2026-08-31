using DeliveryApp.Core.Domain.Models;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.LocationTest;

public class LocationCreateShould
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    public void BeCorrectWhenParamsAreCorrectOnCreated(byte x, byte y)
    {
        // Arrange

        // Act
        var result = Location.Create(x, y);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.X.Should().Be(x);
        result.Value.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(11, 0)]
    [InlineData(0, 11)]
    [InlineData(11, 11)]
    [InlineData(15, 5)]
    [InlineData(5, 15)]
    public void ReturnErrorWhenXOrYIsGreaterThanMaxValue(byte x, byte y)
    {
        // Arrange

        // Act
        var result = Location.Create(x, y);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }
}