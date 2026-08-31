using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Errs;

namespace DeliveryApp.Core.Domain.Models;

/// <summary>
///     Координата на доске
/// </summary>
public class Location : ValueObject
{
    private const byte MinValue = 0;
    private const byte MaxValue = 10;
    
    /// <summary>
    ///     Ctr
    /// </summary>
    [ExcludeFromCodeCoverage]
    private Location()
    {
    }
    
    /// <summary>
    ///     Ctr
    /// </summary>
    /// <param name="x">x (горизонталь), 0..10</param>
    /// <param name="y">y (вертикаль), 0..10</param>
    private Location(byte x, byte y) : this()
    {
        X = x;
        Y = y;
    }
    
    /// <summary>
    ///     X (горизонталь)
    /// </summary>
    public byte X { get; private set; }
    
    /// <summary>
    ///     Y (вертикаль)
    /// </summary>
    public byte Y { get; private set; }

    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="x">x (горизонталь), 0..10</param>
    /// <param name="y">y (вертикаль), 0..10</param>
    /// <returns>Результат</returns>
    public static Result<Location, Error> Create(byte x, byte y)
    {
        if (x > MaxValue) return GeneralErrors.ValueMustBeLessOrEqual(nameof(X), x,  MaxValue);
        if (y > MaxValue) return GeneralErrors.ValueMustBeLessOrEqual(nameof(Y), y,  MaxValue);

        return new Location(x, y);
    }
    
    /// <summary>
    ///     Вычисляет расстояние между двумя точками на сетке.
    /// </summary>
    /// <param name="l">Первая точка</param>
    /// <param name="r">Вторая точка</param>
    /// <returns>Дистанция между точками</returns>
    public static byte GetDistance(Location l, Location r)
    {
        var deltaX = Math.Abs(l.X - r.X);
        var deltaY = Math.Abs(l.Y - r.Y);
        var distance = deltaX + deltaY;
        
        return (byte)distance;
    }
    
    /// <summary>
    ///     Перегрузка для определения идентичности
    /// </summary>
    /// <returns>Результат</returns>
    /// <remarks>Идентичность будет происходить по совокупности полей указанных в методе</remarks>
    [ExcludeFromCodeCoverage]
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}
