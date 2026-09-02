using DeliveryApp.Core.Domain.Models;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.SharedKernel.LocationVoTest;

public class LocationVoCreateShould
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(1, 10)]
    [InlineData(10, 1)]
    public void BeCorrectWhenParamsAreCorrectOnCreated(byte x, byte y)
    {
        // Arrange

        // Act
        var result = LocationVo.Create(x, y);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.X.Should().Be(x);
        result.Value.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(11, 1)]
    [InlineData(1, 11)]
    [InlineData(11, 11)]
    [InlineData(15, 5)]
    [InlineData(5, 15)]
    public void ReturnErrorWhenXOrYIsGreaterThanMaxValue(byte x, byte y)
    {
        // Arrange

        // Act
        var result = LocationVo.Create(x, y);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    public void ReturnErrorWhenXOrYIsLessThanMinValue(byte x, byte y)
    {
        // Arrange

        // Act
        var result = LocationVo.Create(x, y);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }
}
