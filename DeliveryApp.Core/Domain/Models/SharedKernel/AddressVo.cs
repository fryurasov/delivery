using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Errs;

namespace DeliveryApp.Core.Domain.Models.SharedKernel;

public class AddressVo : ValueObject
{
    /// <summary>
    ///     Ctr
    /// </summary>
    [ExcludeFromCodeCoverage]
    private AddressVo()
    {
    }
    
    /// <summary>
    ///     Ctr
    /// </summary>
    private AddressVo(string country, string city, string street, string house, string apartment)
    {
        Country = country;
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
    }
    
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
    ///     Factory Method
    /// </summary>
    /// <param name="country">Страна</param>
    /// <param name="city">Город</param>
    /// <param name="street">Улица</param>
    /// <param name="house">Дом</param>
    /// <param name="apartment">Квартира</param>
    /// <returns>Результат</returns>
    public static Result<AddressVo, Error> Create(string country, string city, string street, string house, string apartment)
    {
        if (string.IsNullOrWhiteSpace(country)) return GeneralErrors.ValueIsRequired(nameof(country));
        if (string.IsNullOrWhiteSpace(city)) return GeneralErrors.ValueIsRequired(nameof(city));
        if (string.IsNullOrWhiteSpace(street)) return GeneralErrors.ValueIsRequired(nameof(street));
        if (string.IsNullOrWhiteSpace(house)) return GeneralErrors.ValueIsRequired(nameof(house));
        if (string.IsNullOrWhiteSpace(apartment)) return GeneralErrors.ValueIsRequired(nameof(apartment));

        return new AddressVo(country, city, street, house, apartment);
    }
    
    /// <summary>
    ///     Перегрузка для определения идентичности
    /// </summary>
    /// <returns>Результат</returns>
    /// <remarks>Идентичность будет происходить по совокупности полей указанных в методе</remarks>
    [ExcludeFromCodeCoverage]
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Country;
        yield return City;
        yield return Street;
        yield return House;
        yield return Apartment;
    }
}