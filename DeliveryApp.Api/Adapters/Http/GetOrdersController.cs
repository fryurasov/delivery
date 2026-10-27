using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.Http;

public class GetOrdersController : GetOrdersApiController
{
    private readonly IMediator _mediator;

    public GetOrdersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<IActionResult> GetOrders()
    {
        var response = await _mediator.Send(GetNotCompletedOrdersQuery.Create());

        var orders = response.Orders
            .Select(o => new Order
            {
                Id = o.Id,
                Location = new Location { X = o.Location.X, Y = o.Location.Y }
            })
            .ToList();

        return Ok(orders);
    }
}
