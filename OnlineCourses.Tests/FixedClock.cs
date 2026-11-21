using OnlineCourses.Core.Abstractions;

namespace OnlineCourses.Tests;

internal sealed class FixedClock : IClock
{
    public FixedClock(DateOnly today) => Today = today;

    public DateOnly Today { get; }
}
