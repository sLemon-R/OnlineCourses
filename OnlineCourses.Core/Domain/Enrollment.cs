namespace OnlineCourses.Core.Domain;

/// <summary>Заявка слухача на навчання. Агрегат: позиції й стан змінюються лише методами.</summary>
public sealed class Enrollment
{
    private readonly List<EnrollmentLine> _lines = new();

    /// <summary>Створює нову заявку в стані New.</summary>
    /// <param name="id">Номер заявки, більший за нуль.</param>
    /// <param name="listenerId">Ідентифікатор слухача, більший за нуль.</param>
    /// <param name="createdAt">Дата створення.</param>
    /// <exception cref="ArgumentOutOfRangeException">Номер чи слухач не додатні.</exception>
    public Enrollment(int id, int listenerId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(listenerId);
        Id = id;
        ListenerId = listenerId;
        CreatedAt = createdAt;
        Status = EnrollmentStatus.New;
    }

    /// <summary>Отримує номер заявки.</summary>
    public int Id { get; }

    /// <summary>Отримує ідентифікатор слухача.</summary>
    public int ListenerId { get; }

    /// <summary>Отримує дату створення.</summary>
    public DateOnly CreatedAt { get; }

    /// <summary>Отримує стан заявки.</summary>
    public EnrollmentStatus Status { get; private set; }

    /// <summary>Отримує позиції заявки лише для читання.</summary>
    public IReadOnlyList<EnrollmentLine> Lines => _lines;

    /// <summary>Додає позицію. Дозволено лише в стані New.</summary>
    /// <param name="line">Позиція заявки.</param>
    /// <exception cref="InvalidOperationException">Заявка вже не в стані New.</exception>
    public void AddLine(EnrollmentLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (Status != EnrollmentStatus.New)
        {
            throw new InvalidOperationException("Позиції можна додавати лише до нової заявки.");
        }

        _lines.Add(line);
    }

    /// <summary>Переводить заявку в інший стан за таблицею дозволених переходів.</summary>
    /// <param name="to">Цільовий стан.</param>
    /// <exception cref="InvalidOperationException">Перехід заборонений.</exception>
    public void ChangeStatus(EnrollmentStatus to)
    {
        if (!CanChangeTo(to))
        {
            throw new InvalidOperationException(
                "Перехід " + Status + " -> " + to + " заборонений.");
        }

        Status = to;
    }

    /// <summary>Перевіряє, чи дозволений перехід. Нічого не змінює.</summary>
    /// <param name="to">Цільовий стан.</param>
    /// <returns>true, якщо перехід дозволений.</returns>
    public bool CanChangeTo(EnrollmentStatus to) => (Status, to) switch
    {
        (EnrollmentStatus.New, EnrollmentStatus.Paid) => _lines.Count > 0,
        (EnrollmentStatus.New, EnrollmentStatus.Cancelled) => true,
        (EnrollmentStatus.Paid, EnrollmentStatus.InProgress) => true,
        (EnrollmentStatus.Paid, EnrollmentStatus.Cancelled) => true,
        (EnrollmentStatus.InProgress, EnrollmentStatus.Completed) => true,
        _ => false,
    };
}
