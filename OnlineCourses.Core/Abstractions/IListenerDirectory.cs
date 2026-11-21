using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Abstractions;

/// <summary>Довідник слухачів.</summary>
public interface IListenerDirectory
{
    /// <summary>Шукає слухача за ідентифікатором.</summary>
    /// <param name="id">Ідентифікатор слухача.</param>
    /// <returns>Слухач або null.</returns>
    Listener? Find(int id);

    /// <summary>Повертає всіх слухачів довідника.</summary>
    /// <returns>Слухачі в порядку додавання.</returns>
    IReadOnlyList<Listener> All();
}
