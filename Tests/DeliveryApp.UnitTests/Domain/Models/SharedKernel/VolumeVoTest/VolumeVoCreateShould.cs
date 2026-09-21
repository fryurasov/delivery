using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.SharedKernel.VolumeVoTest;

public class VolumeVoCreateShould
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void BeCorrectWhenParamsAreCorrectOnCreated(int value)
    {
        // Arrange

        // Act
        var result = VolumeVo.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
        result.Value.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ReturnErrorWhenValueIsLessThanMinValue(int value)
    {
        // Arrange

        // Act
        var result = VolumeVo.Create(value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }
}
