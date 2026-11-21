using System.Text.Json;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Infrastructure;

/// <summary>Перетворює групи на JSON і назад; з файлами не працює.</summary>
public sealed class GroupJsonSerializer
{
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    /// <summary>Записує групи в рядок JSON.</summary>
    /// <param name="groups">Групи для збереження.</param>
    /// <returns>Текст JSON.</returns>
    public string ToJson(IEnumerable<CourseGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);
        List<GroupRecord> records = groups.Select(ToRecord).ToList();
        return JsonSerializer.Serialize(records, _options);
    }

    /// <summary>Відновлює групи з рядка JSON разом із зарахованими слухачами.</summary>
    /// <param name="json">Текст JSON, записаний методом ToJson.</param>
    /// <returns>Відновлені групи.</returns>
    /// <exception cref="JsonException">Текст не є коректним JSON.</exception>
    public IReadOnlyList<CourseGroup> FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        List<GroupRecord> records = JsonSerializer.Deserialize<List<GroupRecord>>(json, _options) ?? new List<GroupRecord>();
        return records.Select(ToGroup).ToList();
    }

    private static GroupRecord ToRecord(CourseGroup group) => new()
    {
        Code = group.Code,
        CourseCode = group.Course.Code,
        Title = group.Course.Title,
        Price = group.Course.Price,
        Capacity = group.Capacity,
        StartDate = group.StartDate,
        Prerequisites = group.Course.Prerequisites.ToList(),
        Listeners = group.Listeners.OrderBy(id => id).ToList(),
    };

    private static CourseGroup ToGroup(GroupRecord record)
    {
        Course course = new()
        {
            Code = record.CourseCode,
            Title = record.Title,
            Price = record.Price,
            Prerequisites = record.Prerequisites,
        };
        CourseGroup group = new(record.Code, course, record.Capacity, record.StartDate);
        foreach (int listenerId in record.Listeners)
        {
            group.Add(listenerId);
        }

        return group;
    }
}
