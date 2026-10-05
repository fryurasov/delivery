using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DeliveryApp.UnitTests.Application.Commands;

public class CreateOrderHandlerShould
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderAggregateRepository _orderRepository;
    private readonly CreateOrderCommandHandler _createOrderCommandHandler;

    public CreateOrderHandlerShould()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepository = Substitute.For<IOrderAggregateRepository>();
        _createOrderCommandHandler = new CreateOrderCommandHandler(_unitOfWork, _orderRepository);
    }

    [Fact]
    public async Task CreateOrderSuccessfullyWhenOrderExists()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        const int volume = 10;
        const string country = "First";
        const string city = "Second";
        const string street = "Test Street";
        const string house = "Test House";
        const string apartment = "Test Apartment";
        
        var command = CreateOrderCommand.Create(orderId, country, city, street, house, apartment, volume).Value;
        
        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        var result = await _createOrderCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _orderRepository.Received(1).AddAsync(Arg.Any<OrderAggregate>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public void CreateOrder_WithError_WhenVolumeIsInvalid()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        const int volume = -1;
        const string country = "First";
        const string city = "Second";
        const string street = "Test Street";
        const string house = "Test House";
        const string apartment = "Test Apartment";

        // Act
        var command = CreateOrderCommand.Create(orderId, country, city, street, house, apartment, volume);

        // Assert
        command.IsSuccess.Should().BeFalse();
    }
}
