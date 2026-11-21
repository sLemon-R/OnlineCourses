namespace OnlineCourses.Core.Domain;

/// <summary>Заявка слухача на навчання.</summary>
public sealed class Enrollment
{
    private readonly List<EnrollmentLine> _lines = new();

    /// <summary>Отримує номер заявки.</summary>
    public int Id { get; init; }

    /// <summary>Отримує ідентифікатор слухача.</summary>
    public int ListenerId { get; init; }

    /// <summary>Отримує або задає стан заявки.</summary>
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.New;

    /// <summary>Отримує дату й час створення заявки.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Отримує рядки заявки.</summary>
    public IReadOnlyList<EnrollmentLine> Lines => _lines;

    /// <summary>Додає рядок до заявки.</summary>
    /// <param name="line">Рядок із курсом, кількістю місць і ціною.</param>
    public void AddLine(EnrollmentLine line) => _lines.Add(line);

    /// <summary>Обчислює вартість заявки.</summary>
    /// <returns>Сума всіх рядків заявки, грн.</returns>
    public decimal Total()
    {
        decimal sum = 0m;
        foreach (EnrollmentLine line in _lines)
        {
            sum += line.Amount;
        }

        return sum;
    }
}
