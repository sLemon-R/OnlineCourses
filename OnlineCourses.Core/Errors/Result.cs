namespace OnlineCourses.Core.Errors;

/// <summary>Результат операції: значення або пояснення очікуваної помилки.</summary>
/// <typeparam name="T">Тип значення.</typeparam>
public sealed class Result<T>
{
    /// <summary>Створює результат.</summary>
    /// <param name="isSuccess">Чи операція вдалася.</param>
    /// <param name="value">Значення при успіху.</param>
    /// <param name="error">Пояснення при невдачі.</param>
    internal Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>Отримує значення, чи операція вдалася.</summary>
    public bool IsSuccess { get; }

    /// <summary>Отримує значення; null, якщо операція не вдалася.</summary>
    public T? Value { get; }

    /// <summary>Отримує пояснення помилки для користувача; порожнє при успіху.</summary>
    public string Error { get; }
}
