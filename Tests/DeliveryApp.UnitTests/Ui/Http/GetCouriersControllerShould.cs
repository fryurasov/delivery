using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeliveryApp.Api.Adapters.Http;
using DeliveryApp.Core.Application.Queries.Common.Dto;
using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OpenApi.Models;
using Xunit;

namespace DeliveryApp.UnitTests.Ui.Http;

public class GetCouriersControllerShould
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    [Fact]
    public async Task ReturnOkWithMappedCouriers()
    {
        // Arrange
        var courierId = Guid.NewGuid();
        _mediator.Send(Arg.Any<GetAllCouriersQuery>()).Returns(new GetAllCouriersResponse
        {
            Couriers = new List<CourierDto>
            {
                new() { Id = courierId, Name = "Test", Location = new LocationDto { X = 3, Y = 5 } }
            }
        });

        var controller = new GetCouriersController(_mediator);

        // Act
        var result = await controller.GetCouriers();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var couriers = okResult.Value.Should().BeAssignableTo<List<Courier>>().Subject;
        couriers.Should().ContainSingle(c => c.Id == courierId && c.Name == "Test" &&
                                              c.Location.X == 3 && c.Location.Y == 5);
    }
}
