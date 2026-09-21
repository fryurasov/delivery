using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.SharedKernel.VolumeVoTest;

public class VolumeVoLessOperatorShould
{
    [Fact]
    public void BeFalseWhenFirstValueIsGreater()
    {
        // Arrange
        var volume1 = VolumeVo.Create(42).Value;
        var volume2 = VolumeVo.Create(0).Value;

        // Act
        var operatorResult = volume1 < volume2;

        // Assert
        operatorResult.Should().BeFalse();
    }
    
    [Fact]
    public void BeTrueWhenFirstValueIsLess()
    {
        // Arrange
        var volume1 = VolumeVo.Create(0).Value;
        var volume2 = VolumeVo.Create(42).Value;

        // Act
        var operatorResult = volume1 < volume2;

        // Assert
        operatorResult.Should().BeTrue();
    }
    
    [Fact]
    public void BeFalseWhenValuesAreSame()
    {
        // Arrange
        var volume1 = VolumeVo.Create(42).Value;
        var volume2 = VolumeVo.Create(42).Value;

        // Act
        var operatorResult = volume1 < volume2;

        // Assert
        operatorResult.Should().BeFalse();
    }
    
    [Fact]
    public void BeFalseWhenSameInstance()
    {
        // Arrange
        var volume = VolumeVo.Create(42).Value;

        // Act
        var operatorResult = volume < volume;

        // Assert
        operatorResult.Should().BeFalse();
    }
}
