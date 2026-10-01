using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Domain.Services.Complete;
using DeliveryApp.Core.Ports;
using Errs;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.CompleteOrder;

public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, UnitResult<Error>>
{
    private ICourierAggregateRepository _courierAggregateRepository;
    private IOrderAggregateRepository _orderAggregateRepository;
    private ICompleteService _completeService;
    private IUnitOfWork _unitOfWork;

    public CompleteOrderCommandHandler(ICourierAggregateRepository courierAggregateRepository, ICompleteService completeService,
        IOrderAggregateRepository orderAggregateRepository, IUnitOfWork unitOfWork)
    {
        _courierAggregateRepository = courierAggregateRepository;
        _orderAggregateRepository = orderAggregateRepository;
        _unitOfWork = unitOfWork;
        _completeService = completeService;
    }
    
    public async Task<UnitResult<Error>> Handle(CompleteOrderCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var getCourierResult = await _courierAggregateRepository.GetByIdAsync(command.CourierId, cancellationToken);
        if (!getCourierResult.HasValue)
            return GeneralErrors.NotFound(nameof(CourierAggregate), command.CourierId);
        var courier = getCourierResult.Value;
        
        var getOrderResult = await _orderAggregateRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (!getOrderResult.HasValue)
            return GeneralErrors.NotFound(nameof(OrderAggregate), command.OrderId);
        var order = getOrderResult.Value;

        var completeResult = _completeService.Complete(order, courier); 
        if (completeResult.IsFailure) 
            return completeResult.Error;
        
        _courierAggregateRepository.Update(courier);
        _orderAggregateRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        await _unitOfWork.CommitTransactionAsync(cancellationToken);
        
        return UnitResult.Success<Error>();
    }   
}