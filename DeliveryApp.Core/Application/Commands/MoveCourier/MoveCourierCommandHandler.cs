using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Ports;
using Errs;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.MoveCourier;

public class MoveCourierCommandHandler : IRequestHandler<MoveCourierCommand, UnitResult<Error>>
{
    private readonly ICourierAggregateRepository _courierAggregateRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    /// <summary>
    ///     Ctr
    /// </summary>
    public MoveCourierCommandHandler(IUnitOfWork unitOfWork, ICourierAggregateRepository courierAggregateRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _courierAggregateRepository = courierAggregateRepository ?? throw new ArgumentNullException(nameof(courierAggregateRepository));
    }
    
    public async Task<UnitResult<Error>> Handle(MoveCourierCommand command, CancellationToken cancellationToken)
    {
        var getCourierResult = await _courierAggregateRepository.GetByIdAsync(command.CourierId, cancellationToken);
        if (!getCourierResult.HasValue)
            return GeneralErrors.NotFound(nameof(CourierAggregate), command.CourierId);

        var courier = getCourierResult.Value;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        var moveResult = courier.Move(command.TargetLocation);
        if (moveResult.IsFailure)
            return moveResult.Error;
        
        _courierAggregateRepository.Update(courier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _unitOfWork.CommitTransactionAsync(cancellationToken);
        
        return UnitResult.Success<Error>();
    }
}