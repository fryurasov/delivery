using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeliveryApp.Api.Adapters.Http;
using DeliveryApp.Core.Application.Queries.Common.Dto;
using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;
using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders.Response;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OpenApi.Models;
using Xunit;

namespace DeliveryApp.UnitTests.Ui.Http;

public class GetOrdersControllerShould
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    [Fact]
    public async Task ReturnOkWithMappedOrders()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mediator.Send(Arg.Any<GetNotCompletedOrdersQuery>()).Returns(new GetNotCompletedOrdersResponse
        {
            Orders = new List<OrderDto>
            {
                new() { Id = orderId, Location = new LocationDto { X = 2, Y = 4 } }
            }
        });

        var controller = new GetOrdersController(_mediator);

        // Act
        var result = await controller.GetOrders();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var orders = okResult.Value.Should().BeAssignableTo<List<Order>>().Subject;
        orders.Should().ContainSingle(o => o.Id == orderId && o.Location.X == 2 && o.Location.Y == 4);
    }
}
