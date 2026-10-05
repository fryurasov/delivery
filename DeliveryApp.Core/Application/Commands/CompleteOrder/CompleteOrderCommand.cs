using CSharpFunctionalExtensions;
using Errs;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.CompleteOrder;

public class CompleteOrderCommand : IRequest<UnitResult<Error>>
{
    /// <summary>
    ///     Идентификатор курьера
    /// </summary>
    public Guid CourierId { get; private set; }
    
    /// <summary>
    ///     Идентификатор заказа
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    ///     Ctr
    /// </summary>
    private CompleteOrderCommand(Guid courierId, Guid orderId)
    {
        OrderId = orderId;
        CourierId = courierId;
    }

    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="courierId"></param>
    /// <param name="orderId"></param>
    public static Result<CompleteOrderCommand, Error> Create(Guid courierId, Guid orderId)
    {
        if (courierId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(courierId));
        if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));

        return new CompleteOrderCommand(courierId, orderId);
    }
}