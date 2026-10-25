using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.Http;

public class CreateOrderController : CreateOrderApiController
{
    private readonly IMediator _mediator;

    public CreateOrderController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<IActionResult> CreateOrder(NewOrder newOrder)
    {
        var createCommandResult = CreateOrderCommand.Create(
            newOrder.Id,
            newOrder.Address.Country,
            newOrder.Address.City,
            newOrder.Address.Street,
            newOrder.Address.House,
            newOrder.Address.Apartment,
            newOrder.Volume);
        if (createCommandResult.IsFailure)
            return BadRequest(createCommandResult.Error);

        var sendResult = await _mediator.Send(createCommandResult.Value);
        if (sendResult.IsFailure)
            return Conflict(sendResult.Error);

        return Created(
            uri: $"/api/v1/orders/{sendResult.Value}",
            value: new CreateOrderResponse { OrderId = sendResult.Value });
    }
}
