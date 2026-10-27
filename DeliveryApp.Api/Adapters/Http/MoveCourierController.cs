using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.MoveCourier;
using Errs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.Http;

public class MoveCourierController : MoveCourierApiController
{
    private readonly IMediator _mediator;

    public MoveCourierController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<IActionResult> MoveCourier(Guid courierId, Location location)
    {
        if (location.X is < byte.MinValue or > byte.MaxValue ||
            location.Y is < byte.MinValue or > byte.MaxValue)
            return BadRequest(GeneralErrors.ValueIsInvalid(nameof(location), $"{location.X},{location.Y}"));

        var createCommandResult = MoveCourierCommand.Create(courierId, (byte)location.X, (byte)location.Y);
        if (createCommandResult.IsFailure)
            return BadRequest(createCommandResult.Error);

        var sendResult = await _mediator.Send(createCommandResult.Value);
        if (sendResult.IsFailure)
            return Conflict(sendResult.Error);

        return Ok();
    }
}
