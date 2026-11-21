using System.Globalization;
using System.Text;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Reporting;

/// <summary>Форматування заявок у CSV. Нічого не зберігає й не друкує.</summary>
public static class EnrollmentCsvReport
{
    /// <summary>Будує текст CSV з роздільником «;».</summary>
    /// <param name="enrollments">Заявки.</param>
    /// <returns>Текст CSV із заголовком.</returns>
    public static string TextOf(IReadOnlyList<Enrollment> enrollments)
    {
        ArgumentNullException.ThrowIfNull(enrollments);
        StringBuilder text = new();
        text.AppendLine("id;status;lines");
        foreach (Enrollment enrollment in enrollments)
        {
            text.AppendLine(string.Format(
                CultureInfo.InvariantCulture,
                "{0};{1};{2}",
                enrollment.Id,
                enrollment.Status,
                enrollment.Lines.Count));
        }

        return text.ToString();
    }
}
