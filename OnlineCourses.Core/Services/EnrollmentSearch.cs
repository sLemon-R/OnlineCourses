using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Services;

/// <summary>UC-04: пошук заявок за слухачем і станом.</summary>
public static class EnrollmentSearch
{
    /// <summary>Відбирає заявки, що відповідають обом критеріям; null означає «будь-який».</summary>
    /// <param name="enrollments">Усі заявки.</param>
    /// <param name="listenerId">Ідентифікатор слухача або null.</param>
    /// <param name="status">Стан заявки або null.</param>
    /// <returns>Заявки, упорядковані за номером.</returns>
    public static IReadOnlyList<Enrollment> Find(IEnumerable<Enrollment> enrollments, int? listenerId, EnrollmentStatus? status)
    {
        ArgumentNullException.ThrowIfNull(enrollments);
        return enrollments
            .Where(e => listenerId is null || e.ListenerId == listenerId)
            .Where(e => status is null || e.Status == status)
            .OrderBy(e => e.Id)
            .ToList();
    }
}
