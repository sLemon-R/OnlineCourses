using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Abstractions;

/// <summary>Сховище навчальних груп. Не знає, де саме лежать дані.</summary>
public interface IGroupRepository
{
    /// <summary>Шукає групу за кодом без урахування регістру.</summary>
    /// <param name="code">Код групи.</param>
    /// <returns>Група або null, якщо такої немає.</returns>
    CourseGroup? Find(string code);

    /// <summary>Повертає всі групи каталогу.</summary>
    /// <returns>Групи в порядку додавання.</returns>
    IReadOnlyList<CourseGroup> All();

    /// <summary>Додає нову групу або замінює наявну з тим самим кодом.</summary>
    /// <param name="group">Група для збереження.</param>
    void Save(CourseGroup group);
}
