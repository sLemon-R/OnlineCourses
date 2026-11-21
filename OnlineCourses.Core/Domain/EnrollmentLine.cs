namespace OnlineCourses.Core.Domain;

/// <summary>Рядок заявки: курс, кількість місць і ціна за місце.</summary>
public sealed class EnrollmentLine
{
    public string CourseCode { get; init; } = string.Empty;

    public int Seats { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal Amount => Seats * UnitPrice;
}
