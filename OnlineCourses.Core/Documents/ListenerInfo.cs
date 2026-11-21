namespace OnlineCourses.Core.Documents;

/// <summary>Дані слухача, потрібні для розрахунку вартості.</summary>
public sealed record ListenerInfo
{
    /// <summary>Отримує прізвище слухача.</summary>
    public required string Name { get; init; }

    /// <summary>Отримує пошту слухача.</summary>
    public required string Email { get; init; }

    /// <summary>Отримує значення, чи слухач постійний.</summary>
    public bool IsReturning { get; init; }
}
