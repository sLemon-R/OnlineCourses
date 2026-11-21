using System.Globalization;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

namespace OnlineCourses.Core.Input;

/// <summary>Межа між рядками з консолі чи файлу та позицією заявки.</summary>
public static class EnrollmentLineParser
{
    /// <summary>Найдовший допустимий код курсу.</summary>
    public const int MaxCodeLength = 20;

    /// <summary>Найбільша кількість місць в одній позиції.</summary>
    public const int MaxSeats = 1000;

    /// <summary>Розбирає рядок «код;місця;ціна» у позицію заявки.</summary>
    /// <param name="raw">Сирий рядок; може бути null чи порожнім.</param>
    /// <returns>Готова позиція або пояснення, що саме не так.</returns>
    public static Result<EnrollmentLine> ParseRow(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Fail<EnrollmentLine>("Порожній рядок");
        }

        string[] parts = raw.Split(';');
        return parts.Length == 3
            ? Parse(parts[0], parts[1], parts[2])
            : Result.Fail<EnrollmentLine>("Потрібно три поля через ;");
    }

    /// <summary>Розбирає код курсу, кількість місць і ціну.</summary>
    /// <param name="courseCode">Код курсу, до 20 символів.</param>
    /// <param name="seats">Кількість місць рядком, від 1 до 1000.</param>
    /// <param name="unitPrice">Ціна рядком, дробова частина через крапку, не менше 0.</param>
    /// <returns>Готова позиція або пояснення помилки.</returns>
    public static Result<EnrollmentLine> Parse(string courseCode, string seats, string unitPrice)
    {
        string code = courseCode?.Trim() ?? string.Empty;
        if (code.Length is 0 or > MaxCodeLength)
        {
            return Result.Fail<EnrollmentLine>("Код курсу порожній або задовгий");
        }

        if (!int.TryParse(seats, NumberStyles.None, CultureInfo.InvariantCulture, out int count)
            || count is < 1 or > MaxSeats)
        {
            return Result.Fail<EnrollmentLine>("Кількість місць — ціле число від 1 до 1000");
        }

        bool isNumber = decimal.TryParse(
            unitPrice, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal price);
        return isNumber
            ? Result.Ok(new EnrollmentLine(code, count, price))
            : Result.Fail<EnrollmentLine>("Ціна — число з крапкою, не менше 0");
    }

    /// <summary>Рахує рядки з додатною кількістю місць без винятків на сміття.</summary>
    /// <param name="rows">Рядки з кількістю місць.</param>
    /// <returns>Кількість коректних рядків.</returns>
    public static int CountValidSeats(IEnumerable<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        int valid = 0;
        foreach (string row in rows)
        {
            if (int.TryParse(row, NumberStyles.Integer, CultureInfo.InvariantCulture, out int seats)
                && seats > 0)
            {
                valid++;
            }
        }

        return valid;
    }
}
