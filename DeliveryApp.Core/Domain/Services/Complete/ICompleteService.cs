using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using Errs;

namespace DeliveryApp.Core.Domain.Services.Complete;

public interface ICompleteService
{
    public UnitResult<Error> Complete(OrderAggregate order, CourierAggregate courier);
}