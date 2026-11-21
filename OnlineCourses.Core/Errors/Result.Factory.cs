namespace OnlineCourses.Core.Errors;

/// <summary>Створення результатів без вказування типу вручну.</summary>
public static class Result
{
    /// <summary>Успішний результат.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="value">Значення.</param>
    /// <returns>Результат зі значенням.</returns>
    public static Result<T> Ok<T>(T value) => new(true, value, string.Empty);

    /// <summary>Невдалий результат з поясненням.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="error">Пояснення для користувача.</param>
    /// <returns>Результат без значення.</returns>
    public static Result<T> Fail<T>(string error) => new(false, default, error);
}
