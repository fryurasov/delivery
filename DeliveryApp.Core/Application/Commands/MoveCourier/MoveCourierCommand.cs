using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.MoveCourier;

public class MoveCourierCommand : IRequest<UnitResult<Error>>
{
    /// <summary>
    ///     Идентификатор курьера
    /// </summary>
    public Guid CourierId { get; private set; }
    
    /// <summary>
    ///     Позиция для перемещения
    /// </summary>
    public LocationVo TargetLocation { get; private set; }

    private MoveCourierCommand(Guid courierId, LocationVo targetLocation)
    {
        CourierId = courierId;
        TargetLocation = targetLocation;
    }

    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="courierId">Идентификатор курьера</param>
    /// <param name="x">X, 1..10</param>
    /// <param name="y">Y, 1..10</param>
    /// <returns>Результат</returns>
    public static Result<MoveCourierCommand, Error> Create(Guid courierId, byte x, byte y)
    {
        if (courierId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(courierId));

        var locationResult = LocationVo.Create(x, y);
        if (locationResult.IsFailure)
            return locationResult.Error;
            
        return new MoveCourierCommand(courierId, locationResult.Value);
    }
}
