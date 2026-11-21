using System.Globalization;
using System.Text;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Reporting;

/// <summary>UC-05: звіт «наповнюваність груп» станом на дату.</summary>
public static class GroupFillReport
{
    private const string Header = "Група        Курс                       Місць  Зайнято  Наповн.";

    /// <summary>Будує рядки звіту для груп, що стартують не раніше за дату.</summary>
    /// <param name="groups">Усі групи каталогу.</param>
    /// <param name="from">Найраніша дата старту, яку включає звіт.</param>
    /// <returns>Рядки від найбільш до найменш заповненої групи, далі за кодом.</returns>
    public static IReadOnlyList<GroupFillRow> Rows(IEnumerable<CourseGroup> groups, DateOnly from)
    {
        ArgumentNullException.ThrowIfNull(groups);
        return groups
            .Where(g => g.StartDate >= from)
            .Select(g => new GroupFillRow
            {
                GroupCode = g.Code,
                CourseTitle = g.Course.Title,
                Capacity = g.Capacity,
                Occupied = g.Occupied,
                FillPercent = g.FillPercent,
            })
            .OrderByDescending(r => r.FillPercent)
            .ThenBy(r => r.GroupCode, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>Форматує рядки звіту в текстову таблицю.</summary>
    /// <param name="rows">Рядки звіту.</param>
    /// <returns>Таблиця з заголовком; для порожнього списку — повідомлення про відсутність груп.</returns>
    public static string Text(IReadOnlyList<GroupFillRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        if (rows.Count == 0)
        {
            return "Груп для звіту немає" + Environment.NewLine;
        }

        StringBuilder text = new();
        text.AppendLine(Header);
        foreach (GroupFillRow row in rows)
        {
            text.AppendLine(string.Format(
                CultureInfo.InvariantCulture,
                "{0,-12} {1,-26} {2,5} {3,8} {4,7:F1} %",
                row.GroupCode,
                row.CourseTitle,
                row.Capacity,
                row.Occupied,
                row.FillPercent));
        }

        return text.ToString();
    }
}
