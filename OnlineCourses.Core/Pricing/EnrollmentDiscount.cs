namespace OnlineCourses.Core.Pricing;

/// <summary>Знижка на заявку: постійному слухачу і за велику заявку.</summary>
public static class EnrollmentDiscount
{
    /// <summary>Ставка знижки постійному слухачу.</summary>
    public const decimal ReturningRate = 0.05m;

    /// <summary>Додаткова ставка за велику заявку.</summary>
    public const decimal BigEnrollmentRate = 0.10m;

    /// <summary>Сума, з якої заявка вважається великою, грн.</summary>
    public const decimal BigEnrollmentFrom = 1000m;

    /// <summary>Рахує знижку на заявку.</summary>
    /// <param name="amount">Сума заявки без знижок, грн.</param>
    /// <param name="isReturning">Чи слухач постійний.</param>
    /// <returns>Знижка, грн, округлена до копійок.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Сума від'ємна.</exception>
    public static decimal For(decimal amount, bool isReturning)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума не може бути від'ємною.");
        }

        decimal rate = 0m;
        if (isReturning)
        {
            rate += ReturningRate;
        }

        if (amount >= BigEnrollmentFrom)
        {
            rate += BigEnrollmentRate;
        }

        return decimal.Round(amount * rate, 2);
    }
}
