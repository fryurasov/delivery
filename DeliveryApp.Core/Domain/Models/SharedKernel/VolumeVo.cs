using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Errs;

namespace DeliveryApp.Core.Domain.Models.SharedKernel;

/// <summary>
///     Объем
/// </summary>
public class VolumeVo : ValueObject
{
    /// <summary>
    ///     Ctr
    /// </summary>
    [ExcludeFromCodeCoverage]
    private VolumeVo()
    {
    }
    
    /// <summary>
    ///     Ctr
    /// </summary>
    /// <param name="value">Объем</param>
    private VolumeVo(int value) : this()
    {
        Value = value;
    }
    
    /// <summary>
    ///     Объем
    /// </summary>
    public int Value { get; private set; }
    
    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <returns>Результат</returns>
    public static Result<VolumeVo, Error> Create(int value)
    {
        if (value < 0) return GeneralErrors.ValueMustBeGreaterOrEqual(nameof(Value), value,  0);

        return new VolumeVo(value);
    }
    
    /// <summary>
    ///     Перегрузка "меньше"
    /// </summary>
    /// <param name="first">Объем 1</param>
    /// <param name="second">Объем 2</param>
    /// <returns>Результат</returns>
    public static bool operator <(VolumeVo first, VolumeVo second)
    {
        var result = first.Value < second.Value;
        return result;
    }

    /// <summary>
    ///     Перегрузка "больше"
    /// </summary>
    /// <param name="first">Объем 1</param>
    /// <param name="second">Объем 2</param>
    /// <returns>Результат</returns>
    public static bool operator >(VolumeVo first, VolumeVo second)
    {
        var result = first.Value > second.Value;
        return result;
    }

    /// <summary>
    ///     Перегрузка "меньше или равно"
    /// </summary>
    /// <param name="first">Объем 1</param>
    /// <param name="second">Объем 2</param>
    /// <returns>Результат</returns>
    public static bool operator <=(VolumeVo first, VolumeVo second)
    {
        var result = first.Value <= second.Value;
        return result;
    }

    /// <summary>
    ///     Перегрузка "больше или равно"
    /// </summary>
    /// <param name="first">Объем 1</param>
    /// <param name="second">Объем 2</param>
    /// <returns>Результат</returns>
    public static bool operator >=(VolumeVo first, VolumeVo second)
    {
        var result = first.Value >= second.Value;
        return result;
    }
    
    /// <summary>
    ///     Перегрузка для определения идентичности
    /// </summary>
    /// <returns>Результат</returns>
    /// <remarks>Идентичность будет происходить по совокупности полей указанных в методе</remarks>
    [ExcludeFromCodeCoverage]
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}
