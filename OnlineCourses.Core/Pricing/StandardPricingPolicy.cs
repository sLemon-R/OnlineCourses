using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Pricing;

/// <summary>Ціна без знижок.</summary>
public sealed class StandardPricingPolicy : IPricingPolicy
{
    /// <inheritdoc/>
    public decimal PriceOf(EnrollmentLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount, 2, MidpointRounding.AwayFromZero);
    }
}
