namespace OnlineCourses.Core.Pricing;

/// <summary>Знижка за пакет курсів в одній заявці з надбавкою постійному слухачу.</summary>
public static class PackageDiscount
{
    /// <summary>Кількість курсів, з якої діє знижка за два курси.</summary>
    public const int TwoCourses  =  2;

    /// <summary>Кількість курсів, з якої діє найбільша знижка.</summary>
    public const int ThreeCourses = 3;

    /// <summary>Ставка знижки за два курси.</summary>
    public const decimal TwoCoursesRate = 0.05m;

    /// <summary>Ставка знижки за три й більше курсів.</summary>
    public const decimal ThreeCoursesRate = 0.10m;

    /// <summary>Надбавка до ставки для постійного слухача.</summary>
    public const decimal ReturningBonus = 0.03m;

    /// <summary>Найбільша знижка на одну заявку, грн.</summary>
    public const decimal MaxDiscount = 1500m;

    /// <summary>Визначає ставку знижки за таблицею рішень.</summary>
    /// <param name="courseCount">Кількість курсів у заявці, більша за нуль.</param>
    /// <param name="isReturning">Чи слухач уже навчався на платформі.</param>
    /// <returns>Ставка від 0 до 0,13.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Курсів не більше нуля.</exception>
    public static decimal RateFor(int courseCount, bool isReturning)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(courseCount);
        decimal rate = courseCount switch
        {
            >= ThreeCourses => ThreeCoursesRate,
            TwoCourses => TwoCoursesRate,
            _ => 0m,
        };
        return isReturning ? rate + ReturningBonus : rate;
    }

    /// <summary>Рахує знижку в гривнях, округлену до копійок від нуля, не більше за стелю.</summary>
    /// <param name="subtotal">Сума курсів без знижки, не від'ємна.</param>
    /// <param name="courseCount">Кількість курсів у заявці.</param>
    /// <param name="isReturning">Чи слухач постійний.</param>
    /// <returns>Знижка, грн.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Сума від'ємна або курсів не більше нуля.</exception>
    public static decimal DiscountFor(decimal subtotal, int courseCount, bool isReturning)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(subtotal);
        decimal raw = subtotal * RateFor(courseCount, isReturning);
        return Math.Min(Math.Round(raw, 2, MidpointRounding.AwayFromZero), MaxDiscount);
    }
}
