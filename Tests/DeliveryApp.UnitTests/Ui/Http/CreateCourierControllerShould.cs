using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Api.Adapters.Http;
using DeliveryApp.Core.Application.Commands.CreateCourier;
using Errs;
using Error = Errs.Error;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OpenApi.Models;
using Xunit;

namespace DeliveryApp.UnitTests.Ui.Http;

public class CreateCourierControllerShould
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    [Fact]
    public async Task ReturnCreatedWhenCourierIsValid()
    {
        // Arrange
        var courierId = Guid.NewGuid();
        _mediator.Send(Arg.Any<CreateCourierCommand>()).Returns(Result.Success<Guid, Error>(courierId));

        var controller = new CreateCourierController(_mediator);

        // Act
        var result = await controller.CreateCourier(new NewCourier { Name = "Test" });

        // Assert
        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task ReturnBadRequestWhenNameIsEmpty()
    {
        // Arrange
        var controller = new CreateCourierController(_mediator);

        // Act
        var result = await controller.CreateCourier(new NewCourier { Name = "" });

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ReturnConflictWhenHandlerFails()
    {
        // Arrange
        _mediator.Send(Arg.Any<CreateCourierCommand>())
            .Returns(Result.Failure<Guid, Error>(GeneralErrors.ValueIsRequired("name")));

        var controller = new CreateCourierController(_mediator);

        // Act
        var result = await controller.CreateCourier(new NewCourier { Name = "Test" });

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }
}
