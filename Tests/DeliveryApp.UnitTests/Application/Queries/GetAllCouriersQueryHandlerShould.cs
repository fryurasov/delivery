using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.Queries.Common.Dto;
using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DeliveryApp.UnitTests.Application.Queries;

public class GetAllCouriersQueryHandlerShould
{
    private readonly ICourierQueryService _courierQueryService;
    private readonly GetAllCouriersQueryHandler _getAllCouriersQueryHandler;

    public GetAllCouriersQueryHandlerShould()
    {
        _courierQueryService = Substitute.For<ICourierQueryService>();
        _getAllCouriersQueryHandler = new GetAllCouriersQueryHandler(_courierQueryService);
    }

    [Fact]
    public async Task ReturnEmptyResponseWhenNoCouriersExist()
    {
        // Arrange
        var query = new GetAllCouriersQuery();

        var couriersResponseEmpty = new GetAllCouriersResponse() { Couriers = new List<CourierDto>() };
        _courierQueryService.GetAllCouriers().Returns(Task.FromResult(couriersResponseEmpty));

        // Act
        var result = await _getAllCouriersQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Couriers.Should().NotBeNull();
        result.Couriers.Should().BeEmpty();
    }
    
    [Fact]
    public async Task ReturnSingleCourier_WhenOneCourierExists()
    {
        // Arrange
        var query = new GetAllCouriersQuery();
        var courierId = Guid.NewGuid();
        
        var couriersResponse = new GetAllCouriersResponse()
        {
            Couriers = new List<CourierDto>()
            {
                new CourierDto()
                {
                    Id = courierId,
                    Name = "Test1",
                    Location = new LocationDto() { X = 3, Y = 5 }
                }
            }
        };

        _courierQueryService.GetAllCouriers().Returns(Task.FromResult(couriersResponse));

        // Act
        var result = await _getAllCouriersQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Couriers.Should().HaveCount(1);
        result.Couriers.First().Should().BeEquivalentTo(
            new {
                Id = courierId,
                Name = "Test1",
                Location = new { X = 3, Y = 5 }
            }
        );
        await _courierQueryService.Received(1).GetAllCouriers();
    }

}