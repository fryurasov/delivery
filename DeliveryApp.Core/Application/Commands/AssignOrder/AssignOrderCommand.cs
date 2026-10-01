using CSharpFunctionalExtensions;
using Errs;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.AssignOrder;

public class AssignOrderCommand : IRequest<UnitResult<Error>>
{
    /// <summary>
    ///     Ctr
    /// </summary>
    private AssignOrderCommand() {}
    
    /// <summary>
    ///     Factory Method
    /// </summary>
    public static AssignOrderCommand Create()
    {
        return new AssignOrderCommand();
    }
}
