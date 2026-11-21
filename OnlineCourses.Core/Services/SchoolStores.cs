using OnlineCourses.Core.Abstractions;

namespace OnlineCourses.Core.Services;

/// <summary>Три сховища, які завжди потрібні сервісу зарахування разом.</summary>
public sealed class SchoolStores
{
    /// <summary>Створює набір сховищ.</summary>
    /// <param name="groups">Сховище груп.</param>
    /// <param name="listeners">Довідник слухачів.</param>
    /// <param name="enrollments">Сховище заявок.</param>
    /// <exception cref="ArgumentNullException">Будь-яке сховище не задано.</exception>
    public SchoolStores(IGroupRepository groups, IListenerDirectory listeners, IEnrollmentRepository enrollments)
    {
        ArgumentNullException.ThrowIfNull(groups);
        ArgumentNullException.ThrowIfNull(listeners);
        ArgumentNullException.ThrowIfNull(enrollments);
        Groups = groups;
        Listeners = listeners;
        Enrollments = enrollments;
    }

    /// <summary>Отримує сховище груп.</summary>
    public IGroupRepository Groups { get; }

    /// <summary>Отримує довідник слухачів.</summary>
    public IListenerDirectory Listeners { get; }

    /// <summary>Отримує сховище заявок.</summary>
    public IEnrollmentRepository Enrollments { get; }
}
