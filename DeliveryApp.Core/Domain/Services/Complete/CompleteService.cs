using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using Errs;

namespace DeliveryApp.Core.Domain.Services.Complete;

public class CompleteService : ICompleteService
{
    public UnitResult<Error> Complete(OrderAggregate order, CourierAggregate courier)
    {
        var courierCompleteResult = courier.Complete(order);
        if (courierCompleteResult.IsFailure)
            return courierCompleteResult.Error;
        
        var orderCompleteResult = order.Complete();
        if (orderCompleteResult.IsFailure)
            return orderCompleteResult.Error;
        
        return UnitResult.Success<Error>();
    }   
}