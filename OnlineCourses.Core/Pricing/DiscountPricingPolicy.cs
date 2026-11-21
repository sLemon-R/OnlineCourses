using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Pricing;

/// <summary>Ціна зі знижкою заданою часткою.</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private const decimal MaxRate = 0.5m;

    private readonly decimal _rate;

    /// <summary>Створює політику зі знижкою.</summary>
    /// <param name="rate">Частка знижки від 0 до 0,5.</param>
    /// <exception cref="ArgumentOutOfRangeException">Частка поза межами.</exception>
    public DiscountPricingPolicy(decimal rate)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rate, MaxRate);
        _rate = rate;
    }

    /// <inheritdoc/>
    public decimal PriceOf(EnrollmentLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - _rate), 2, MidpointRounding.AwayFromZero);
    }
}
