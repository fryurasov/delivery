using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Api.Adapters.Http;
using DeliveryApp.Core.Application.Commands.CompleteOrder;
using Errs;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace DeliveryApp.UnitTests.Ui.Http;

public class CompleteOrderControllerShould
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly Guid _courierId = Guid.NewGuid();
    private readonly Guid _orderId = Guid.NewGuid();

    [Fact]
    public async Task ReturnOkWhenCompleteIsSuccessful()
    {
        // Arrange
        _mediator.Send(Arg.Any<CompleteOrderCommand>()).Returns(UnitResult.Success<Error>());
        var controller = new CompleteOrderController(_mediator);

        // Act
        var result = await controller.CompleteOrder(_courierId, _orderId);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task ReturnConflictWhenHandlerFails()
    {
        // Arrange
        _mediator.Send(Arg.Any<CompleteOrderCommand>())
            .Returns(UnitResult.Failure(GeneralErrors.NotFound(nameof(_courierId), _courierId)));
        var controller = new CompleteOrderController(_mediator);

        // Act
        var result = await controller.CompleteOrder(_courierId, _orderId);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }
}
