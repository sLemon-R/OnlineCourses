namespace OnlineCourses.Core.Documents;

/// <summary>Перевірки передумов для розрахунку вартості заявки.</summary>
public static class EnrollmentGuards
{
    /// <summary>Перевіряє номер заявки й контакти слухача.</summary>
    /// <param name="enrollmentId">Номер заявки, більший за нуль.</param>
    /// <param name="listenerName">Прізвище слухача.</param>
    /// <param name="listenerEmail">Пошта слухача із символом @.</param>
    /// <exception cref="ArgumentOutOfRangeException">Номер заявки не додатний.</exception>
    /// <exception cref="ArgumentException">Ім'я чи пошта порожні або пошта без @.</exception>
    public static void EnsureHeaderValid(
        int enrollmentId,
        string listenerName,
        string listenerEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(enrollmentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(listenerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(listenerEmail);
        if (!listenerEmail.Contains('@', StringComparison.Ordinal))
        {
            throw new ArgumentException("Пошта має містити символ @.", nameof(listenerEmail));
        }
    }

    /// <summary>Перевіряє одну позицію заявки.</summary>
    /// <param name="line">Позиція з кодом курсу, місцями й ціною.</param>
    /// <exception cref="ArgumentNullException">Позиція відсутня.</exception>
    /// <exception cref="ArgumentException">Код курсу порожній.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Місць не більше нуля або ціна від'ємна.
    /// </exception>
    public static void EnsureLineValid(CourseItem line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.CourseCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(line.Seats, nameof(line.Seats));
        ArgumentOutOfRangeException.ThrowIfNegative(line.UnitPrice, nameof(line.UnitPrice));
    }

    /// <summary>Перевіряє, що заявка має хоча б одну коректну позицію.</summary>
    /// <param name="lines">Позиції заявки.</param>
    /// <exception cref="ArgumentNullException">Список позицій відсутній.</exception>
    /// <exception cref="ArgumentException">Список позицій порожній.</exception>
    public static void EnsureLinesValid(IReadOnlyList<CourseItem> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException("Заявка не містить жодної позиції.", nameof(lines));
        }

        foreach (CourseItem line in lines)
        {
            EnsureLineValid(line);
        }
    }

    /// <summary>Перевіряє весь запит на розрахунок одним викликом.</summary>
    /// <param name="request">Запит на розрахунок вартості.</param>
    /// <exception cref="ArgumentNullException">Запит або слухач відсутні.</exception>
    /// <exception cref="ArgumentException">Заголовок чи позиції некоректні.</exception>
    public static void EnsureRequestValid(TotalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Listener);
        EnsureHeaderValid(request.EnrollmentId, request.Listener.Name, request.Listener.Email);
        EnsureLinesValid(request.Lines);
        ArgumentOutOfRangeException.ThrowIfNegative(request.DeliveryPrice);
    }
}
