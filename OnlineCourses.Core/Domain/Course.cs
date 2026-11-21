namespace OnlineCourses.Core.Domain;

/// <summary>Курс у каталозі навчальної платформи.</summary>
public sealed class Course
{
    /// <summary>Отримує код курсу.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Отримує назву курсу.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Отримує ціну одного місця, грн.</summary>
    public decimal Price { get; init; }

    /// <summary>Отримує тривалість курсу в годинах.</summary>
    public int DurationHours { get; init; }

    /// <summary>Отримує кількість вільних місць.</summary>
    public int SeatsLeft { get; init; }

    /// <summary>Отримує коди курсів, які треба пройти до цього курсу.</summary>
    public IReadOnlyList<string> Prerequisites { get; init; } = Array.Empty<string>();
}
