using System.Globalization;
using System.Text;
using OnlineCourses.Core.Documents;

namespace OnlineCourses.Core.Reporting;

/// <summary>Формування тексту звіту про заявку.</summary>
public static class EnrollmentReport
{
    /// <summary>Будує текст звіту. Чиста функція: нічого не друкує й не змінює.</summary>
    /// <param name="request">Запит, за яким рахували вартість.</param>
    /// <param name="result">Результат розрахунку.</param>
    /// <returns>Багаторядковий текст звіту.</returns>
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);

        // явна культура: інакше роздільник у сумі залежить від налаштувань ОС
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine(string.Format(culture, "Заявка № {0}", request.EnrollmentId));
        text.AppendLine(string.Format(culture, "Слухач: {0}", request.Listener.Name));
        foreach (CourseItem line in request.Lines)
        {
            text.AppendLine(string.Format(culture, "{0} x {1}", line.CourseCode, line.Seats));
        }

        text.AppendLine(string.Format(culture, "Разом: {0:F2}", result.Total));
        text.AppendLine(string.Format(culture, "Стан: {0}", result.Status));
        return text.ToString();
    }
}
