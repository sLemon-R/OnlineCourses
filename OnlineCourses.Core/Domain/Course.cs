namespace OnlineCourses.Core.Domain;

/// <summary>Курс у каталозі навчальної платформи.</summary>
public sealed class Course
{
    public string Code { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int DurationHours { get; init; }

    public int SeatsLeft { get; init; }
}
