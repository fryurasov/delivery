using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAggregateTest;

public class CourierAggregateCreateShould
{
    [Fact]
    public void BeSuccessWhenParametersAreValid()
    {
        // Arrange
        var name = "Иван";
        var location = LocationVo.Create(5, 5).Value;

        // Act
        var result = CourierAggregate.Create(name, location);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Location.Should().Be(location);
        result.Value.AssignmentsAsReadOnly.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ReturnErrorWhenNameIsInvalid(string invalidName)
    {
        // Arrange
        var location = LocationVo.Create(5, 5).Value;

        // Act
        var result = CourierAggregate.Create(invalidName, location);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}