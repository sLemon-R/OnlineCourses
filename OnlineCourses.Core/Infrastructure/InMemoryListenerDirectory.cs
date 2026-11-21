using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Infrastructure;

/// <summary>Довідник слухачів у пам'яті.</summary>
public sealed class InMemoryListenerDirectory : IListenerDirectory
{
    private readonly Dictionary<int, Listener> _items = new();

    /// <summary>Додає слухача або замінює наявного з тим самим ідентифікатором.</summary>
    /// <param name="listener">Слухач.</param>
    public void Add(Listener listener)
    {
        ArgumentNullException.ThrowIfNull(listener);
        _items[listener.Id] = listener;
    }

    /// <inheritdoc/>
    public Listener? Find(int id) => _items.TryGetValue(id, out Listener? found) ? found : null;

    /// <inheritdoc/>
    public IReadOnlyList<Listener> All() => _items.Values.ToList();
}
