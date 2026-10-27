using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using OpenApi.Models;
using Xunit;

namespace DeliveryApp.ComponentTests;

public class OrderControllerShould : ComponentTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static NewOrder BuildValidOrder(Guid id) => new()
    {
        Id = id,
        Volume = 5,
        Address = new Address
        {
            Country = "Russia",
            City = "Moscow",
            Street = "Tverskaya",
            House = "1",
            Apartment = "1"
        }
    };

    [Fact]
    public async Task ReturnCreatedWhenOrderIsValid()
    {
        // Arrange
        var newOrder = BuildValidOrder(Guid.NewGuid());

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/orders", newOrder, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CreateOrderResponse>(JsonOptions);
        body!.OrderId.Should().Be(newOrder.Id);
    }

    [Fact]
    public async Task ReturnBadRequestWhenAddressIsMissing()
    {
        // Arrange
        var newOrder = new NewOrder { Id = Guid.NewGuid(), Volume = 5, Address = null! };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/orders", newOrder, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReturnOrdersListContainingCreatedOrder_WhenGetOrdersCalled()
    {
        // Arrange
        var newOrder = BuildValidOrder(Guid.NewGuid());
        await Client.PostAsJsonAsync("/api/v1/orders", newOrder, JsonOptions);

        // Act
        var response = await Client.GetAsync("/api/v1/orders/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<Order>>(JsonOptions);
        orders.Should().Contain(o => o.Id == newOrder.Id);
    }
}
