using System.Text;

namespace OnlineCourses.Core.Reporting;

/// <summary>Формує текст простого звіту із заголовка й готових рядків.</summary>
public sealed class ReportBuilder
{
    /// <summary>Отримує заголовок звіту; друкується великими літерами.</summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>Складає текст звіту: заголовок, потім кожен рядок з нового рядка.</summary>
    /// <param name="rows">Рядки звіту в потрібному порядку.</param>
    /// <returns>Готовий текст звіту.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        StringBuilder text = new();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (string row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }

    /// <summary>Повертає підпис із кількістю позицій.</summary>
    /// <param name="count">Кількість позицій.</param>
    /// <returns>Текст підпису.</returns>
    public string Describe(int count)
    {
        string note;
        return "Позицій: " + count;
    }
}
