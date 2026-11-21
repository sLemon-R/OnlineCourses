using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Documents;

/// <summary>Результат розрахунку: вартість і наступний стан заявки.</summary>
public sealed record TotalResult
{
    /// <summary>Отримує вартість заявки, грн.</summary>
    public decimal Total { get; init; }

    /// <summary>Отримує наступний стан заявки.</summary>
    public EnrollmentStatus Status { get; init; }
}
