using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.CreateCourier;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.Http;

public class CreateCourierController : CreateCourierApiController
{
    private readonly IMediator _mediator;

    public CreateCourierController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<IActionResult> CreateCourier(NewCourier newCourier)
    {
        var createCommandResult = CreateCourierCommand.Create(newCourier.Name);
        if (createCommandResult.IsFailure)
            return BadRequest(createCommandResult.Error);

        var sendResult = await _mediator.Send(createCommandResult.Value);
        if (sendResult.IsFailure)
            return Conflict(sendResult.Error);

        return Created(
            uri: $"/api/v1/couriers/{sendResult.Value}",
            value: new CreateCourierResponse { CourierId = sendResult.Value });
    }
}
