using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using Errs;

namespace DeliveryApp.Core.Domain.Services.Dispatch;

public class DispatchService : IDispatchService
{
    public Result<CourierAggregate, Error> Dispatch(OrderAggregate order, IEnumerable<CourierAggregate> couriers)
    {
        if (order.Status != OrderStatusVo.Created)
            return Errors.OrderIsNotCreated();

        var bestCourier = couriers
            .Where(c => c.CanAcceptOrder(order) is { IsSuccess: true, Value: true })
            .Select(c => new { Courier = c, DistanceResult = c.GetDistanceToOrder(order) })
            .Where(x => x.DistanceResult.IsSuccess)
            .MinBy(x => x.DistanceResult.Value)
            ?.Courier;

        if (bestCourier == null)
            return Errors.CourierNotFound();

        order.Assign(bestCourier);
        bestCourier.AssignOrder(order);
        
        return bestCourier;
    }

    public static class Errors
    {
        public static Error OrderIsNotCreated() => 
            new("order.is.not.created", "Назначить курьера можно только на заказ в статусе \"Создан\"");

        public static Error CourierNotFound() => 
            new("courier.not.found", "Не удалось найти подходящего курьера");
    }
}
