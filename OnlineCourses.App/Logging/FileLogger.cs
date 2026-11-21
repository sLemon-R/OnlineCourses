using System.Globalization;
using Microsoft.Extensions.Logging;

namespace OnlineCourses.App.Logging;

/// <summary>Журнал однієї категорії: рядок «час рівень категорія повідомлення».</summary>
internal sealed class FileLogger : ILogger
{
    private readonly string _category;
    private readonly StreamWriter _writer;
    private readonly object _gate;

    /// <summary>Створює журнал категорії.</summary>
    /// <param name="category">Назва категорії, зазвичай ім'я класу.</param>
    /// <param name="writer">Спільний потік файлу.</param>
    /// <param name="gate">Об'єкт блокування для запису з кількох потоків.</param>
    public FileLogger(string category, StreamWriter writer, object gate)
    {
        _category = category;
        _writer = writer;
        _gate = gate;
    }

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string line = string.Format(
            CultureInfo.InvariantCulture,
            "{0:yyyy-MM-dd HH:mm:ss} {1,-11} {2} {3}{4}",
            DateTime.Now,
            logLevel,
            _category,
            formatter(state, exception),
            exception is null ? string.Empty : " | " + exception.GetType().Name + ": " + exception.Message);
        lock (_gate)
        {
            _writer.WriteLine(line);
        }
    }
}
