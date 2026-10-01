using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Models.Order;

namespace DeliveryApp.Core.Ports;

public interface IOrderAggregateRepository : IRepository<OrderAggregate>
{
    Task AddAsync(OrderAggregate order);
    void Update(OrderAggregate order);
    Task<Maybe<OrderAggregate>> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Получить один любой новый заказ (в статусе Created)
    /// </summary>
    Task<Maybe<OrderAggregate>> GetFirstCreatedAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Получить все назначенные заказы (в статусе Assigned)
    /// </summary>
    Task<List<OrderAggregate>> GetAssignedAsync(CancellationToken cancellationToken);
    
    public Task<bool> IsExistByIdAsync(Guid id, CancellationToken cancellationToken);
}