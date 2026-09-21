using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.SharedKernel.VolumeVoTest;

public class VolumeVoEqualOperatorShould
{
    [Fact]
    public void BeEqualWhenValuesAreSame()
    {
        // Arrange
        var volume1 = VolumeVo.Create(10).Value;
        var volume2 = VolumeVo.Create(10).Value;

        // Act
        var result = volume1.Equals(volume2);
        var operatorResult = volume1 == volume2;

        // Assert
        result.Should().BeTrue();
        operatorResult.Should().BeTrue();
    }
    
    [Fact]
    public void BeEqualWhenSameInstance()
    {
        // Arrange
        var volume = VolumeVo.Create(10).Value;

        // Act
        var selfEquals = volume.Equals(volume);
        var operatorResult = volume == volume;

        // Assert
        selfEquals.Should().BeTrue();
        operatorResult.Should().BeTrue();
    }
    
    [Fact]
    public void BeNoEqualityIfTheValuesAreNotEqual()
    {
        // Arrange
        var volume1 = VolumeVo.Create(10).Value;
        var volume2 = VolumeVo.Create(12).Value;

        // Act
        var selfEquals = volume1.Equals(volume2);
        var operatorResult = volume1 == volume2;

        // Assert
        selfEquals.Should().BeFalse();
        operatorResult.Should().BeFalse();
    }
}
