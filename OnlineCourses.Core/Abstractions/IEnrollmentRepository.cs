using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Abstractions;

/// <summary>Сховище заявок. Відповідає лише за зберігання.</summary>
public interface IEnrollmentRepository
{
    /// <summary>Додає або замінює заявку.</summary>
    /// <param name="enrollment">Заявка.</param>
    void Add(Enrollment enrollment);

    /// <summary>Шукає заявку за номером.</summary>
    /// <param name="id">Номер заявки.</param>
    /// <returns>Заявка або null.</returns>
    Enrollment? GetById(int id);

    /// <summary>Повертає всі заявки.</summary>
    /// <returns>Заявки в порядку додавання.</returns>
    IReadOnlyList<Enrollment> GetAll();
}
