using System.Globalization;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

namespace OnlineCourses.App;

/// <summary>Барикада: розбирає сирі рядки з консолі у позицію заявки.</summary>
internal static class EnrollmentLineParser
{
    /// <summary>Розбирає код курсу, кількість місць і ціну.</summary>
    /// <param name="courseCode">Код курсу.</param>
    /// <param name="seats">Кількість місць рядком.</param>
    /// <param name="unitPrice">Ціна рядком, дробова частина через крапку.</param>
    /// <returns>Готова позиція або пояснення помилки.</returns>
    public static Result<EnrollmentLine> Parse(string courseCode, string seats, string unitPrice)
    {
        if (string.IsNullOrWhiteSpace(courseCode))
        {
            return Result.Fail<EnrollmentLine>("Код курсу порожній");
        }

        if (!int.TryParse(seats, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count)
            || count <= 0)
        {
            return Result.Fail<EnrollmentLine>("Кількість місць — ціле число більше 0");
        }

        bool isNumber = decimal.TryParse(
            unitPrice, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal price);
        if (!isNumber
            || price < 0m)
        {
            return Result.Fail<EnrollmentLine>("Ціна — число з крапкою, не менше 0");
        }

        return Result.Ok(new EnrollmentLine(courseCode.Trim(), count, price));
    }
}
