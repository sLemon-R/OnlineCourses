namespace OnlineCourses.Core.Domain;

/// <summary>Слухач онлайн-курсів.</summary>
public sealed class Listener
{
    /// <summary>Отримує ідентифікатор слухача.</summary>
    public int Id { get; init; }

    /// <summary>Отримує прізвище та ім'я слухача.</summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>Отримує електронну пошту слухача.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Отримує значення, чи слухач уже навчався на платформі.</summary>
    public bool IsReturning { get; init; }
}
