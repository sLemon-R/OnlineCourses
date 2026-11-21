namespace OnlineCourses.Core.Domain;

/// <summary>Слухач онлайн-курсів.</summary>
public sealed class Listener
{
    public int Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public bool IsReturning { get; init; }
}
