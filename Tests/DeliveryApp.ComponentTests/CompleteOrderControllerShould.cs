using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DeliveryApp.Core.Application.Commands.AssignOrder;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OpenApi.Models;
using Xunit;

namespace DeliveryApp.ComponentTests;

public class CompleteOrderControllerShould : ComponentTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public async Task ReturnConflictWhenCourierOrOrderDoNotExist()
    {
        // Arrange
        var unknownCourierId = Guid.NewGuid();
        var unknownOrderId = Guid.NewGuid();

        // Act
        var response = await Client.PostAsync(
            $"/api/v1/couriers/{unknownCourierId}/orders/{unknownOrderId}/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ReturnOkWhenOrderGetsAutoAssignedAndThenCompleted()
    {
        // Arrange
        var courierResponse = await Client.PostAsJsonAsync(
            "/api/v1/couriers", new NewCourier { Name = "Auto Assign Courier" }, JsonOptions);
        var courier = await courierResponse.Content.ReadFromJsonAsync<CreateCourierResponse>(JsonOptions);

        var orderId = Guid.NewGuid();
        var orderResponse = await Client.PostAsJsonAsync("/api/v1/orders", new NewOrder
        {
            Id = orderId,
            Volume = 1,
            Address = new Address
            {
                Country = "Russia",
                City = "Moscow",
                Street = "Tverskaya",
                House = "1",
                Apartment = "1"
            }
        }, JsonOptions);
        orderResponse.EnsureSuccessStatusCode();

        using var scope = Factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(AssignOrderCommand.Create());

        // Act
        var completeResponse = await Client.PostAsync(
            $"/api/v1/couriers/{courier!.CourierId}/orders/{orderId}/complete", null);

        // Assert
        completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
