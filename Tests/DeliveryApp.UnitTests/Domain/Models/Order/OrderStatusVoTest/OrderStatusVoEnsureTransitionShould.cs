using DeliveryApp.Core.Domain.Models.Order;
using Errs;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Order.OrderStatusVoTest;

public class OrderStatusVoEnsureTransitionShould
{
    [Theory]
    [MemberData(nameof(GetSameStatusTransitions))]
    public void ReturnAlreadyInStatusErrorWhenTransitionToSameStatus(OrderStatusVo currentStatus)
    {
        // Act
        var result = currentStatus.EnsureCanTransitionTo(currentStatus);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderStatusVo.Errors.AlreadyInStatus(currentStatus));
    }

    [Theory]
    [MemberData(nameof(GetInvalidTransitionsWithSpecificErrors))]
    public void ReturnSpecificErrorWhenTransitionIsInvalid(
        OrderStatusVo currentStatus, 
        OrderStatusVo newStatus, 
        Error expectedError)
    {
        // Act
        var result = currentStatus.EnsureCanTransitionTo(newStatus);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);
    }

    // Строго типизированные наборы данных через TheoryData
    public static TheoryData<OrderStatusVo> GetSameStatusTransitions() => new()
    {
        OrderStatusVo.Created,
        OrderStatusVo.Assigned,
        OrderStatusVo.Completed
    };

    public static TheoryData<OrderStatusVo, OrderStatusVo, Error> GetInvalidTransitionsWithSpecificErrors() => new()
    {
        { 
            OrderStatusVo.Completed, 
            OrderStatusVo.Assigned, 
            OrderStatusVo.Errors.OldStatusNotEqualCreated(OrderStatusVo.Completed) 
        },
        { 
            OrderStatusVo.Created, 
            OrderStatusVo.Completed, 
            OrderStatusVo.Errors.OldStatusNotEqualAssigned(OrderStatusVo.Created) 
        },
        { 
            OrderStatusVo.Completed, 
            OrderStatusVo.Created, 
            OrderStatusVo.Errors.InvalidTransition(OrderStatusVo.Completed, OrderStatusVo.Created) 
        },
        { 
            OrderStatusVo.Assigned, 
            OrderStatusVo.Created, 
            OrderStatusVo.Errors.InvalidTransition(OrderStatusVo.Assigned, OrderStatusVo.Created) 
        }
    };
}