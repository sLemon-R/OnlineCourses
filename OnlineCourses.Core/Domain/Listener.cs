namespace OnlineCourses.Core.Domain;

/// <summary>Слухач онлайн-курсів. Незмінний.</summary>
public sealed record Listener
{
    /// <summary>Створює слухача й перевіряє інваріант.</summary>
    /// <param name="id">Ідентифікатор, більший за нуль.</param>
    /// <param name="fullName">Прізвище та ім'я.</param>
    /// <param name="email">Пошта із символом @.</param>
    /// <param name="isReturning">Чи слухач уже навчався на платформі.</param>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор не додатний.</exception>
    /// <exception cref="ArgumentException">Ім'я порожнє або пошта без @.</exception>
    public Listener(int id, string fullName, string email, bool isReturning)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!email.Contains('@', StringComparison.Ordinal))
        {
            throw new ArgumentException("Пошта має містити символ @.", nameof(email));
        }

        Id = id;
        FullName = fullName;
        Email = email;
        IsReturning = isReturning;
    }

    /// <summary>Отримує ідентифікатор слухача.</summary>
    public int Id { get; }

    /// <summary>Отримує прізвище та ім'я.</summary>
    public string FullName { get; }

    /// <summary>Отримує пошту.</summary>
    public string Email { get; }

    /// <summary>Отримує значення, чи слухач уже навчався на платформі.</summary>
    public bool IsReturning { get; }

    /// <summary>Отримує коди пройдених курсів; зараховуються як передумови.</summary>
    public IReadOnlySet<string> CompletedCourses { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}
