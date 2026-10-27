using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Api.Adapters.Http;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using Errs;
using Error = Errs.Error;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OpenApi.Models;
using Xunit;

namespace DeliveryApp.UnitTests.Ui.Http;

public class CreateOrderControllerShould
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    private static NewOrder ValidOrder() => new()
    {
        Id = Guid.NewGuid(),
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
        var orderId = Guid.NewGuid();
        _mediator.Send(Arg.Any<CreateOrderCommand>()).Returns(Result.Success<Guid, Error>(orderId));

        var controller = new CreateOrderController(_mediator);

        // Act
        var result = await controller.CreateOrder(ValidOrder());

        // Assert
        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task ReturnConflictWhenHandlerFails()
    {
        // Arrange
        _mediator.Send(Arg.Any<CreateOrderCommand>())
            .Returns(Result.Failure<Guid, Error>(GeneralErrors.ValueIsRequired("orderId")));

        var controller = new CreateOrderController(_mediator);

        // Act
        var result = await controller.CreateOrder(ValidOrder());

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }
}
