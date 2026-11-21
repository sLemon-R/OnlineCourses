using OnlineCourses.Core.Abstractions;

namespace OnlineCourses.Core.Infrastructure;

/// <summary>Годинник, що повертає дату з системного часу.</summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc/>
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
