using Ddd;
using DeliveryApp.Core.Domain.Models.Order;

namespace DeliveryApp.Core.Ports;

public interface IOrderAggregateRepository : IRepository<OrderAggregate>
{
    Task AddAsync(OrderAggregate order);
    void Update(OrderAggregate order);
    Task<OrderAggregate?> GetByIdAsync(Guid orderId);
    
    /// <summary>
    /// Получить один любой новый заказ (в статусе Created)
    /// </summary>
    Task<OrderAggregate?> GetFirstCreatedAsync();

    /// <summary>
    /// Получить все назначенные заказы (в статусе Assigned)
    /// </summary>
    Task<List<OrderAggregate>> GetAssignedAsync();
}