using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Errs;

namespace DeliveryApp.Core.Domain.Models.SharedKernel;

/// <summary>
///     Объем
/// </summary>
public class VolumeVo : ValueObject
{
    public static VolumeVo CourierVolumeMax => new VolumeVo(20); 
    
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
    ///     Возвращает сумму объемов из переданной коллекции
    /// </summary>
    /// <param name="volumes">Коллекция объемов</param>
    /// <returns>Суммарный объем</returns>
    public static VolumeVo Sum(IEnumerable<VolumeVo> volumes)
    {
        var total = volumes.Sum(v => v.Value);
        
        return new VolumeVo(total);
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
    ///     Перегрузка "сложения"
    /// </summary>
    /// <param name="first">Объем 1</param>
    /// <param name="second">Объем 2</param>
    /// <returns>Результат</returns>
    public static VolumeVo operator +(VolumeVo first, VolumeVo second)
    {
        var result = first.Value + second.Value;
        
        return new VolumeVo(result);
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
