using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Pricing;

/// <summary>Корпоративна політика: знижка 15 % на позиції від 5 місць.</summary>
public sealed class GroupPricingPolicy : IPricingPolicy
{
    private const int GroupSeats = 5;
    private const decimal GroupRate = 0.15m;

    /// <inheritdoc/>
    public decimal PriceOf(EnrollmentLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        decimal rate = line.Seats >= GroupSeats ? GroupRate : 0m;
        return Math.Round(line.Amount * (1m - rate), 2, MidpointRounding.AwayFromZero);
    }
}
