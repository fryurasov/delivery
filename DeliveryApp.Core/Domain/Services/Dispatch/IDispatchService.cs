using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using Errs;

namespace DeliveryApp.Core.Domain.Services.Dispatch;

public interface IDispatchService
{
    public Result<CourierAggregate, Error> Dispatch(OrderAggregate order, IEnumerable<CourierAggregate> couriers);
}