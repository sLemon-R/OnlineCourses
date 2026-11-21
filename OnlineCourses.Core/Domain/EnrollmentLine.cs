namespace OnlineCourses.Core.Domain;

/// <summary>Позиція заявки: курс, кількість місць і ціна місця. Незмінна.</summary>
public sealed record EnrollmentLine
{
    /// <summary>Створює позицію й перевіряє інваріант.</summary>
    /// <param name="courseCode">Код курсу, не порожній.</param>
    /// <param name="seats">Кількість місць, більша за нуль.</param>
    /// <param name="unitPrice">Ціна одного місця, не від'ємна.</param>
    /// <exception cref="ArgumentException">Код курсу порожній.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Місць не більше нуля або ціна від'ємна.
    /// </exception>
    public EnrollmentLine(string courseCode, int seats, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(courseCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(seats);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);
        CourseCode = courseCode;
        Seats = seats;
        UnitPrice = unitPrice;
    }

    /// <summary>Отримує код курсу.</summary>
    public string CourseCode { get; }

    /// <summary>Отримує кількість місць.</summary>
    public int Seats { get; }

    /// <summary>Отримує ціну одного місця, грн.</summary>
    public decimal UnitPrice { get; }

    /// <summary>Отримує суму позиції без знижок.</summary>
    public decimal Amount => Seats * UnitPrice;
}
