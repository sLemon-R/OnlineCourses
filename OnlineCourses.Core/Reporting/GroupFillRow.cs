namespace OnlineCourses.Core.Reporting;

/// <summary>Рядок звіту «наповнюваність груп».</summary>
public sealed record GroupFillRow
{
    /// <summary>Отримує код групи.</summary>
    public required string GroupCode { get; init; }

    /// <summary>Отримує назву курсу.</summary>
    public required string CourseTitle { get; init; }

    /// <summary>Отримує кількість місць у групі.</summary>
    public required int Capacity { get; init; }

    /// <summary>Отримує кількість зайнятих місць.</summary>
    public required int Occupied { get; init; }

    /// <summary>Отримує наповненість у відсотках.</summary>
    public required decimal FillPercent { get; init; }
}
