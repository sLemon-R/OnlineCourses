using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Abstractions;

/// <summary>Політика ціни позиції заявки.</summary>
public interface IPricingPolicy
{
    /// <summary>Рахує ціну позиції з урахуванням знижки політики.</summary>
    /// <param name="line">Позиція заявки.</param>
    /// <returns>Ціна позиції, округлена до копійок.</returns>
    decimal PriceOf(EnrollmentLine line);
}
