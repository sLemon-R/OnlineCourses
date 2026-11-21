using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Infrastructure;

/// <summary>Сховище груп у пам'яті зі словником за кодом без урахування регістру.</summary>
public sealed class InMemoryGroupRepository : IGroupRepository
{
    private readonly Dictionary<string, CourseGroup> _items = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public CourseGroup? Find(string code) =>
        _items.TryGetValue(code, out CourseGroup? found) ? found : null;

    /// <inheritdoc/>
    public IReadOnlyList<CourseGroup> All() => _items.Values.ToList();

    /// <inheritdoc/>
    public void Save(CourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);
        _items[group.Code] = group;
    }
}
