namespace OnlineCourses.Core.Infrastructure;

/// <summary>Група у форматі файлу JSON.</summary>
public sealed class GroupRecord
{
    /// <summary>Отримує або задає код групи.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Отримує або задає код курсу.</summary>
    public string CourseCode { get; set; } = string.Empty;

    /// <summary>Отримує або задає назву курсу.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Отримує або задає ціну курсу, грн.</summary>
    public decimal Price { get; set; }

    /// <summary>Отримує або задає кількість місць.</summary>
    public int Capacity { get; set; }

    /// <summary>Отримує або задає дату старту.</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>Отримує або задає коди курсів-передумов.</summary>
    public IReadOnlyList<string> Prerequisites { get; set; } = Array.Empty<string>();

    /// <summary>Отримує або задає ідентифікатори зарахованих слухачів.</summary>
    public IReadOnlyList<int> Listeners { get; set; } = Array.Empty<int>();
}
