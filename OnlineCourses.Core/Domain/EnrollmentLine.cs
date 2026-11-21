namespace OnlineCourses.Core.Domain;

/// <summary>Рядок заявки: курс, кількість місць і ціна за місце.</summary>
public sealed class EnrollmentLine
{
    /// <summary>Отримує код курсу.</summary>
    public string CourseCode { get; init; } = string.Empty;

    /// <summary>Отримує кількість місць.</summary>
    public int Seats { get; init; }

    /// <summary>Отримує ціну одного місця, грн.</summary>
    public decimal UnitPrice { get; init; }

    /// <summary>Отримує суму рядка.</summary>
    public decimal Amount => Seats * UnitPrice;
}
