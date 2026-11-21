namespace OnlineCourses.Core.Domain;

/// <summary>Архівна заявка. Не є заявкою: лише показує її дані, змінити нічим.</summary>
public sealed class ArchivedEnrollment
{
    private readonly Enrollment _enrollment;

    /// <summary>Створює архівну копію завершеної або скасованої заявки.</summary>
    /// <param name="enrollment">Заявка в кінцевому стані.</param>
    /// <param name="archivedAt">Дата архівування.</param>
    /// <exception cref="InvalidOperationException">Заявка ще не в кінцевому стані.</exception>
    public ArchivedEnrollment(Enrollment enrollment, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        if (enrollment.Status is not (EnrollmentStatus.Completed or EnrollmentStatus.Cancelled))
        {
            throw new InvalidOperationException("Архівувати можна лише завершену чи скасовану заявку.");
        }

        _enrollment = enrollment;
        ArchivedAt = archivedAt;
    }

    /// <summary>Отримує дату архівування.</summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>Отримує номер заявки.</summary>
    public int Id => _enrollment.Id;

    /// <summary>Отримує стан заявки на момент архівування.</summary>
    public EnrollmentStatus Status => _enrollment.Status;

    /// <summary>Отримує позиції лише для читання.</summary>
    public IReadOnlyList<EnrollmentLine> Lines => _enrollment.Lines;
}
