using System.Globalization;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Infrastructure;

namespace OnlineCourses.App;

/// <summary>Умовні дані каталогу: курси, групи й слухачі, згенеровані кодом.</summary>
internal static class SeedData
{
    private const int ListenerCount = 60;
    private const int ReturningEvery = 3;
    private const int GroupsPerCourse = 2;
    private const int GroupCapacity = 12;
    private const int DaysBetweenStarts = 14;

    /// <summary>Заповнює довідник слухачами, а сховище — групами курсів.</summary>
    /// <param name="listeners">Довідник слухачів.</param>
    /// <param name="groups">Сховище груп.</param>
    /// <param name="firstStart">Дата старту першої групи.</param>
    public static void Fill(InMemoryListenerDirectory listeners, InMemoryGroupRepository groups, DateOnly firstStart)
    {
        for (int id = 1; id <= ListenerCount; id++)
        {
            string number = id.ToString("D2", CultureInfo.InvariantCulture);
            Listener listener = new(id, "Слухач " + number, "listener" + number + "@example.com", id % ReturningEvery == 0)
            {
                CompletedCourses = id % ReturningEvery == 0
                    ? new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CS-101" }
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            };
            listeners.Add(listener);
        }

        int index = 0;
        foreach (Course course in Courses())
        {
            for (int g = 0; g < GroupsPerCourse; g++)
            {
                DateOnly start = firstStart.AddDays(DaysBetweenStarts * index);
                string code = course.Code + "-" + (char)('A' + g);
                groups.Save(new CourseGroup(code, course, GroupCapacity, start));
                index++;
            }
        }
    }

    private static IEnumerable<Course> Courses()
    {
        yield return new Course { Code = "CS-101", Title = "Основи програмування", Price = 2400m };
        yield return new Course { Code = "CS-201", Title = "ООП на C#", Price = 3200m, Prerequisites = new[] { "CS-101" } };
        yield return new Course { Code = "DB-101", Title = "Бази даних і SQL", Price = 2800m };
        yield return new Course { Code = "WEB-101", Title = "HTML, CSS і JavaScript", Price = 2600m };
        yield return new Course { Code = "QA-101", Title = "Тестування ПЗ", Price = 2200m };
        yield return new Course { Code = "AI-201", Title = "Вступ до машинного навчання", Price = 4100m, Prerequisites = new[] { "CS-101" } };
    }
}
