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
    private readonly IGeoClient _geoClient;
    private readonly IOrderAggregateRepository _orderAggregateRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    ///     Ctr
    /// </summary>
    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IOrderAggregateRepository orderAggregateRepository, IGeoClient geoClient)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _orderAggregateRepository = orderAggregateRepository ?? throw new ArgumentNullException(nameof(orderAggregateRepository));
        _geoClient = geoClient ?? throw new ArgumentNullException(nameof(geoClient));
    }

    public async Task<Result<Guid, Error>> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
    {
        var locationResult = await _geoClient.GetGeolocationAsync(message.Address, cancellationToken);
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