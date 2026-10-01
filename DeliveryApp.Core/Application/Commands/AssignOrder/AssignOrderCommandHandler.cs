using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Services.Dispatch;
using DeliveryApp.Core.Ports;
using Errs;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.AssignOrder;

public class AssignOrderCommandHandler : IRequestHandler<AssignOrderCommand, UnitResult<Error>>
{
    private IDispatchService _dispatchService;
    private ICourierAggregateRepository _courierAggregateRepository;
    private IOrderAggregateRepository _orderAggregateRepository;
    private IUnitOfWork _unitOfWork;

    public AssignOrderCommandHandler(IDispatchService dispatchService, ICourierAggregateRepository courierAggregateRepository, 
        IOrderAggregateRepository orderAggregateRepository, IUnitOfWork unitOfWork)
    {
        _dispatchService = dispatchService;
        _courierAggregateRepository = courierAggregateRepository;
        _orderAggregateRepository = orderAggregateRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<UnitResult<Error>> Handle(AssignOrderCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        var getFirstCreatedResult = await _orderAggregateRepository.GetFirstCreatedAsync(cancellationToken);
        if (!getFirstCreatedResult.HasValue)
            return UnitResult.Success<Error>(); 
        var order = getFirstCreatedResult.Value;
        
        var couriers = await _courierAggregateRepository.GetAllAsync(cancellationToken);
        
        var courierDispatchResult = _dispatchService.Dispatch(order, couriers);
        if (courierDispatchResult.IsFailure)
            return courierDispatchResult.ConvertFailure<Error>();
        var courierDispatch = courierDispatchResult.Value;
        
        _courierAggregateRepository.Update(courierDispatch);
        _orderAggregateRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _unitOfWork.CommitTransactionAsync(cancellationToken);
        
        return UnitResult.Success<Error>();
    }
}