using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Api.Adapters.Http;
using DeliveryApp.Core.Application.Commands.MoveCourier;
using Errs;
using Error = Errs.Error;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OpenApi.Models;
using Xunit;

namespace DeliveryApp.UnitTests.Ui.Http;

public class MoveCourierControllerShould
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly Guid _courierId = Guid.NewGuid();

    [Fact]
    public async Task ReturnOkWhenMoveIsValid()
    {
        // Arrange
        _mediator.Send(Arg.Any<MoveCourierCommand>()).Returns(UnitResult.Success<Error>());
        var controller = new MoveCourierController(_mediator);

        // Act
        var result = await controller.MoveCourier(_courierId, new Location { X = 1, Y = 1 });

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task ReturnBadRequestWhenLocationIsOutOfRange()
    {
        // Arrange
        var controller = new MoveCourierController(_mediator);

        // Act
        var result = await controller.MoveCourier(_courierId, new Location { X = -1, Y = 1 });

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ReturnConflictWhenHandlerFails()
    {
        // Arrange
        _mediator.Send(Arg.Any<MoveCourierCommand>())
            .Returns(UnitResult.Failure(GeneralErrors.ValueIsRequired("courierId")));
        var controller = new MoveCourierController(_mediator);

        // Act
        var result = await controller.MoveCourier(_courierId, new Location { X = 1, Y = 1 });

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }
}
