using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using OpenApi.Models;
using Xunit;

namespace DeliveryApp.ComponentTests;

public class CourierControllerShould : ComponentTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public async Task ReturnCreatedWhenCourierIsValid()
    {
        // Arrange
        var newCourier = new NewCourier { Name = "Courier Test" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/couriers", newCourier, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CreateCourierResponse>(JsonOptions);
        body!.CourierId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task ReturnBadRequestWhenNameIsEmpty()
    {
        // Arrange
        var newCourier = new NewCourier { Name = "" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/couriers", newCourier, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReturnCouriersListContainingCreatedCourier_WhenGetCouriersCalled()
    {
        // Arrange
        var newCourier = new NewCourier { Name = "Courier For Get" };
        var createResponse = await Client.PostAsJsonAsync("/api/v1/couriers", newCourier, JsonOptions);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateCourierResponse>(JsonOptions);

        // Act
        var response = await Client.GetAsync("/api/v1/couriers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var couriers = await response.Content.ReadFromJsonAsync<List<Courier>>(JsonOptions);
        couriers.Should().Contain(c => c.Id == created!.CourierId && c.Name == "Courier For Get");
    }

    [Fact]
    public async Task ReturnOkWhenMovingCourierToAdjacentLocation()
    {
        // Arrange
        var newCourier = new NewCourier { Name = "Courier To Move" };
        var createResponse = await Client.PostAsJsonAsync("/api/v1/couriers", newCourier, JsonOptions);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateCourierResponse>(JsonOptions);

        var adjacentLocation = new Location { X = 1, Y = 2 }; // курьер создаётся в (1,1)

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/v1/couriers/{created!.CourierId}/move", adjacentLocation, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReturnConflictWhenMovingCourierToNonAdjacentLocation()
    {
        // Arrange
        var newCourier = new NewCourier { Name = "Courier Far Move" };
        var createResponse = await Client.PostAsJsonAsync("/api/v1/couriers", newCourier, JsonOptions);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateCourierResponse>(JsonOptions);

        var farLocation = new Location { X = 10, Y = 10 };

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/v1/couriers/{created!.CourierId}/move", farLocation, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ReturnConflictWhenMovingUnknownCourier()
    {
        // Arrange
        var unknownCourierId = Guid.NewGuid();
        var location = new Location { X = 1, Y = 2 };

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/v1/couriers/{unknownCourierId}/move", location, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
