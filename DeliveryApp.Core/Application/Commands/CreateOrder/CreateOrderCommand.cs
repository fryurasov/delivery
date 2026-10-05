using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Errs;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Result<Guid, Error>>
{
    /// <summary>
    ///     Идентификатор заказа
    /// </summary>
    public Guid OrderId { get; private set; }
    
    /// <summary>
    ///     Объем заказа
    /// </summary>
    public AddressVo Address { get; private set; }
    
    /// <summary>
    ///     Объем заказа
    /// </summary>
    public VolumeVo Volume  { get; private set; }
    
    /// <summary>
    ///     Ctr
    /// </summary>
    private CreateOrderCommand(Guid orderId, AddressVo address, VolumeVo volume)
    {
        OrderId = orderId;
        Address = address;
        Volume = volume;
    }
    
    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="orderId">Идентификатор корзины</param>
    /// <param name="country">Страна</param>
    /// <param name="city">Город</param>
    /// <param name="street">Улица</param>
    /// <param name="house">Дом</param>
    /// <param name="apartment">Квартира</param>
    /// <param name="volume">Объем заказа</param>
    /// <returns>Результат</returns>
    public static Result<CreateOrderCommand, Error> Create(Guid orderId, string country, string city, string street,
        string house, string apartment, int volume)
    {
        if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
        
        var addressResult = AddressVo.Create(country, city, street, house, apartment);
        if (addressResult.IsFailure) return addressResult.Error;

        var volumeResult = VolumeVo.Create(volume);
        if (volumeResult.IsFailure) return volumeResult.Error;
        
        return new CreateOrderCommand(orderId, addressResult.Value, volumeResult.Value);
    }
}