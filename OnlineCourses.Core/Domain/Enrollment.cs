namespace OnlineCourses.Core.Domain;

/// <summary>Заявка слухача на навчання.</summary>
public sealed class Enrollment
{
    private readonly List<EnrollmentLine> _lines = new();

    public int Id { get; init; }

    public int ListenerId { get; init; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.New;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<EnrollmentLine> Lines => _lines;

    /// <summary>Додає рядок до заявки.</summary>
    public void AddLine(EnrollmentLine line) => _lines.Add(line);

    /// <summary>Обчислює вартість заявки.</summary>
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
