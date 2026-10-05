using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports;
using Errs;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid, Error>>
{
    private readonly IOrderAggregateRepository _orderAggregateRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    /// <summary>
    ///     Ctr
    /// </summary>
    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IOrderAggregateRepository orderAggregateRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _orderAggregateRepository = orderAggregateRepository ?? throw new ArgumentNullException(nameof(orderAggregateRepository));
    }
    
    public async Task<Result<Guid, Error>> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
    {
        var locationResult = LocationVo.Create(1, 1); // Заглушка по ТЗ
        if (locationResult.IsFailure)
        {
            return locationResult.Error;
        }
        
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        var orderCreateResult = OrderAggregate.Create(message.OrderId, locationResult.Value, message.Volume);
        if (orderCreateResult.IsFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return orderCreateResult.Error;
        }

        var order = orderCreateResult.Value;
        
        await _orderAggregateRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        await _unitOfWork.CommitTransactionAsync(cancellationToken);
        
        return order.Id;
    }
}