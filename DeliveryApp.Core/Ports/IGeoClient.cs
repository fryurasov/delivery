using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;

namespace DeliveryApp.Core.Ports;

public interface IGeoClient
{
    /// <summary>
    ///     Получить геопозицию по адресу
    /// </summary>
    Task<Result<LocationVo, Error>> GetGeolocationAsync(AddressVo address, CancellationToken cancellationToken);
}
