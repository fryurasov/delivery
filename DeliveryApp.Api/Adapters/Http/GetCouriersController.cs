using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.Http;

public class GetCouriersController : GetCouriersApiController
{
    private readonly IMediator _mediator;

    public GetCouriersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<IActionResult> GetCouriers()
    {
        var response = await _mediator.Send(GetAllCouriersQuery.Create());

        var couriers = response.Couriers
            .Select(c => new Courier
            {
                Id = c.Id,
                Name = c.Name,
                Location = new Location { X = c.Location.X, Y = c.Location.Y }
            })
            .ToList();

        return Ok(couriers);
    }
}
