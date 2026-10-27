using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.CompleteOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;

namespace DeliveryApp.Api.Adapters.Http;

public class CompleteOrderController : CompleteOrderApiController
{
    private readonly IMediator _mediator;

    public CompleteOrderController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<IActionResult> CompleteOrder(Guid courierId, Guid orderId)
    {
        var createCommandResult = CompleteOrderCommand.Create(courierId, orderId);
        if (createCommandResult.IsFailure)
            return BadRequest(createCommandResult.Error);

        var sendResult = await _mediator.Send(createCommandResult.Value);
        if (sendResult.IsFailure)
            return Conflict(sendResult.Error);

        return Ok();
    }
}
