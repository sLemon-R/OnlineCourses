using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Infrastructure;

/// <summary>Сховище заявок у пам'яті на словнику.</summary>
public sealed class InMemoryEnrollmentRepository : IEnrollmentRepository
{
    private readonly Dictionary<int, Enrollment> _items = new();

    /// <inheritdoc/>
    public void Add(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        _items[enrollment.Id] = enrollment;
    }

    /// <inheritdoc/>
    public Enrollment? GetById(int id) =>
        _items.TryGetValue(id, out Enrollment? found) ? found : null;

    /// <inheritdoc/>
    public IReadOnlyList<Enrollment> GetAll() => _items.Values.ToList();
}
