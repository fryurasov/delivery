using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.CreateCourier;
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
    ///     Страна
    /// </summary>
    public string Country  { get; private set; }
    
    /// <summary>
    ///     Город
    /// </summary>
    public string City { get; private set; } 
    
    /// <summary>
    ///     Улица
    /// </summary>
    public string Street  { get; private set; }
    
    /// <summary>
    ///     Дом
    /// </summary>
    public string House { get; private set; }
    
    /// <summary>
    ///     Квартира
    /// </summary>
    public string Apartment { get; private set; }
    
    /// <summary>
    ///     Объем заказа
    /// </summary>
    public VolumeVo Volume  { get; private set; }
    
    /// <summary>
    ///     Ctr
    /// </summary>
    private CreateOrderCommand(Guid orderId, string country, string city, string street,
        string house, string apartment, VolumeVo volume)
    {
        OrderId = orderId;
        Country = country;
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
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
        if (string.IsNullOrWhiteSpace(country)) return GeneralErrors.ValueIsRequired(nameof(country));
        if (string.IsNullOrWhiteSpace(city)) return GeneralErrors.ValueIsRequired(nameof(city));
        if (string.IsNullOrWhiteSpace(street)) return GeneralErrors.ValueIsRequired(nameof(street));
        if (string.IsNullOrWhiteSpace(house)) return GeneralErrors.ValueIsRequired(nameof(house));
        if (string.IsNullOrWhiteSpace(apartment)) return GeneralErrors.ValueIsRequired(nameof(apartment));

        var volumeResult = VolumeVo.Create(volume);
        if (volumeResult.IsFailure) return volumeResult.Error;
        
        return new CreateOrderCommand(orderId, country, city, street, house, apartment, volumeResult.Value);
    }
}