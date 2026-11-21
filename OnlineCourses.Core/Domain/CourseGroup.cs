using System.Globalization;
using OnlineCourses.Core.Errors;

namespace OnlineCourses.Core.Domain;

/// <summary>Навчальна група курсу з обмеженою кількістю місць.</summary>
public sealed class CourseGroup
{
    private const decimal Percent = 100m;

    private readonly HashSet<int> _listeners = new();

    /// <summary>Створює групу й перевіряє інваріант.</summary>
    /// <param name="code">Код групи, наприклад CS-101-A; не порожній.</param>
    /// <param name="course">Курс, який вивчає група.</param>
    /// <param name="capacity">Кількість місць, більша за нуль.</param>
    /// <param name="startDate">Дата першого заняття.</param>
    /// <exception cref="ArgumentException">Код групи порожній.</exception>
    /// <exception cref="ArgumentNullException">Курс не задано.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Місць не більше нуля.</exception>
    public CourseGroup(string code, Course course, int capacity, DateOnly startDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentNullException.ThrowIfNull(course);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        Code = code;
        Course = course;
        Capacity = capacity;
        StartDate = startDate;
    }

    /// <summary>Отримує код групи, унікальний у каталозі.</summary>
    public string Code { get; }

    /// <summary>Отримує курс, за яким навчається група.</summary>
    public Course Course { get; }

    /// <summary>Отримує загальну кількість місць.</summary>
    public int Capacity { get; }

    /// <summary>Отримує дату першого заняття; після неї зарахування закрите.</summary>
    public DateOnly StartDate { get; }

    /// <summary>Отримує кількість зайнятих місць.</summary>
    public int Occupied => _listeners.Count;

    /// <summary>Отримує кількість вільних місць.</summary>
    public int FreeSeats => Capacity - Occupied;

    /// <summary>Отримує частку зайнятих місць у відсотках, округлену до десятих.</summary>
    public decimal FillPercent => Math.Round((Percent * Occupied) / Capacity, 1, MidpointRounding.AwayFromZero);

    /// <summary>Отримує ідентифікатори слухачів групи.</summary>
    public IReadOnlyCollection<int> Listeners => _listeners;

    /// <summary>Перевіряє, чи слухач уже в групі.</summary>
    /// <param name="listenerId">Ідентифікатор слухача.</param>
    /// <returns>true, якщо слухач зарахований до групи.</returns>
    public bool Contains(int listenerId) => _listeners.Contains(listenerId);

    /// <summary>Займає місце для слухача.</summary>
    /// <param name="listenerId">Ідентифікатор слухача, більший за нуль.</param>
    /// <exception cref="DomainRuleException">Вільних місць немає або слухач уже в групі.</exception>
    public void Add(int listenerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(listenerId);
        if (FreeSeats == 0)
        {
            throw new DomainRuleException("group.full", "У групі " + Code + " немає вільних місць");
        }

        if (!_listeners.Add(listenerId))
        {
            throw new DomainRuleException("group.duplicate", "Слухач " + IdText(listenerId) + " уже в групі " + Code);
        }
    }

    /// <summary>Звільняє місце слухача.</summary>
    /// <param name="listenerId">Ідентифікатор слухача.</param>
    /// <exception cref="DomainRuleException">Слухача немає в групі.</exception>
    public void Remove(int listenerId)
    {
        if (!_listeners.Remove(listenerId))
        {
            throw new DomainRuleException("group.member", "Слухача " + IdText(listenerId) + " немає в групі " + Code);
        }
    }

    private static string IdText(int listenerId) => listenerId.ToString(CultureInfo.InvariantCulture);
}
