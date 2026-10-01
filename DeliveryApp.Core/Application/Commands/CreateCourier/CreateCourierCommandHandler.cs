using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Core.Domain.Models.Courier;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports;
using Errs;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.CreateCourier;

public class CreateCourierCommandHandler : IRequest<Result<CreateCourierCommand, Result<Guid, Error>>>
{
    private readonly ICourierAggregateRepository _courierAggregateRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    /// <summary>
    ///     Ctr
    /// </summary>
    public CreateCourierCommandHandler(IUnitOfWork unitOfWork, ICourierAggregateRepository courierAggregateRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _courierAggregateRepository = courierAggregateRepository ?? throw new ArgumentNullException(nameof(courierAggregateRepository));
    }
    
    public async Task<Result<Guid, Error>> Handle(CreateCourierCommand message, CancellationToken cancellationToken)
    {
        var locationResult = LocationVo.Create(1, 1); // Заглушка по ТЗ
        if (locationResult.IsFailure)
        {
            return locationResult.Error;
        }
        
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        var courierCreateResult = CourierAggregate.Create(message.Name, locationResult.Value);
        if (courierCreateResult.IsFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return courierCreateResult.Error;
        }

        var courier = courierCreateResult.Value;
        
        await _courierAggregateRepository.AddAsync(courier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        await _unitOfWork.CommitTransactionAsync(cancellationToken);
        
        return courier.Id;
    }
}