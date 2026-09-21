using System.Collections.Generic;
using DeliveryApp.Core.Domain.Models.Order;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.Order.OrderStatusVoTest;

public class OrderStatusVoCanTransitionShould
{
    [Theory]
    [MemberData(nameof(GetValidTransitions))]
    public void ReturnTrueWhenTransitionIsValid(OrderStatusVo currentStatus, OrderStatusVo newStatus)
    {
        // Act
        var result = currentStatus.CanTransitionTo(newStatus);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetInvalidTransitions))]
    public void ReturnFalseWhenTransitionIsInvalid(OrderStatusVo currentStatus, OrderStatusVo newStatus)
    {
        // Act
        var result = currentStatus.CanTransitionTo(newStatus);

        // Assert
        result.Should().BeFalse();
    }

    // Возвращаем строго типизированные TheoryData
    public static TheoryData<OrderStatusVo, OrderStatusVo> GetValidTransitions() => new()
    {
        { OrderStatusVo.Created, OrderStatusVo.Assigned },
        { OrderStatusVo.Assigned, OrderStatusVo.Completed }
    };

    public static TheoryData<OrderStatusVo, OrderStatusVo> GetInvalidTransitions() => new()
    {
        // Переход в тот же статус
        { OrderStatusVo.Created, OrderStatusVo.Created },
        { OrderStatusVo.Assigned, OrderStatusVo.Assigned },
        { OrderStatusVo.Completed, OrderStatusVo.Completed },

        // Недопустимые переходы
        { OrderStatusVo.Created, OrderStatusVo.Completed },
        { OrderStatusVo.Assigned, OrderStatusVo.Created },
        { OrderStatusVo.Completed, OrderStatusVo.Created },
        { OrderStatusVo.Completed, OrderStatusVo.Assigned }
    };
}