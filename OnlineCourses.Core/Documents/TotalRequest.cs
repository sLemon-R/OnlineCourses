using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Documents;

/// <summary>Запит на розрахунок вартості заявки.</summary>
public sealed record TotalRequest
{
    /// <summary>Отримує номер заявки.</summary>
    public required int EnrollmentId { get; init; }

    /// <summary>Отримує дані слухача.</summary>
    public required ListenerInfo Listener { get; init; }

    /// <summary>Отримує позиції заявки.</summary>
    public required IReadOnlyList<CourseItem> Lines { get; init; }

    /// <summary>Отримує дату створення заявки.</summary>
    public DateOnly CreatedAt { get; init; }

    /// <summary>Отримує код купона або null.</summary>
    public string? CouponCode { get; init; }

    /// <summary>Отримує поточний стан заявки.</summary>
    public EnrollmentStatus Status { get; init; }

    /// <summary>Отримує ціну надсилання матеріалів, грн.</summary>
    public decimal DeliveryPrice { get; init; }
}
