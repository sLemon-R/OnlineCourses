namespace OnlineCourses.Core.Abstractions;

/// <summary>Джерело поточної дати; у тестах підміняється фіксованою датою.</summary>
public interface IClock
{
    /// <summary>Отримує сьогоднішню дату.</summary>
    DateOnly Today { get; }
}
