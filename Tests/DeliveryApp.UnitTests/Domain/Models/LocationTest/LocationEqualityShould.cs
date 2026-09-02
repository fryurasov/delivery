using DeliveryApp.Core.Domain.Models;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.LocationTest;

public class LocationEqualityShould
{
    [Fact]
    public void BeEqualWhenXAndYAreSame()
    {
        // Arrange
        var location1 = Location.Create(5, 7).Value;
        var location2 = Location.Create(5, 7).Value;

        // Act
        var result = location1.Equals(location2);
        var operatorResult = location1 == location2;

        // Assert
        result.Should().BeTrue();
        operatorResult.Should().BeTrue();
    }
    
    [Fact]
    public void BeEqualWhenSameInstance()
    {
        // Arrange
        var location = Location.Create(5, 7).Value;

        // Act
        var selfEquals = location.Equals(location);
        var operatorResult = location == location;

        // Assert
        selfEquals.Should().BeTrue();
        operatorResult.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, 1, 1, 2)]
    [InlineData(1, 1, 2, 1)]
    [InlineData(1, 2, 1, 1)]
    [InlineData(2, 1, 1, 1)]
    [InlineData(1, 1, 5, 1)]
    public void BeNoEqualityIfTheValuesAreNotEqual(byte x1, byte y1, byte x2, byte y2)
    {
        // Arrange
        var location1 = Location.Create(x1, y1).Value;
        var location2 = Location.Create(x2, y2).Value;

        // Act
        var result = location1.Equals(location2);

        // Assert
        result.Should().BeFalse();
    }
}