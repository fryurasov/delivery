using System;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Courier.CourierAssignmentEntityTest;

public class CourierAssignmentCreateShould
{
    [Fact]
    public void BeCorrectWhenParamsAreCorrectOnCreated()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var location = LocationVo.Create(5, 5).Value;
        var volume = VolumeVo.Create(42).Value;

        // Act
        var result = CourierAssignmentEntity.Create(guid, location, volume);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(CourierAssignmentStatusVo.Assigned);
        result.Value.OrderId.Should().Be(guid);
        result.Value.Location.Should().Be(location);
        result.Value.Volume.Should().Be(volume);
    }
}
