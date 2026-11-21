using System.Globalization;

namespace OnlineCourses.Core.Infrastructure;

/// <summary>Журнал аудиту у файлі: операція, заявка, час. Звільняє файл через Dispose.</summary>
public sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>Відкриває файл журналу для дописування.</summary>
    /// <param name="path">Шлях до файлу.</param>
    public FileAuditLog(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _writer = new StreamWriter(path, append: true);
    }

    /// <summary>Дописує рядок аудиту.</summary>
    /// <param name="operation">Назва операції.</param>
    /// <param name="enrollmentId">Номер заявки.</param>
    /// <exception cref="ObjectDisposedException">Журнал уже закрито.</exception>
    public void Write(string operation, int enrollmentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _writer.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            "{0:O};{1};{2}",
            DateTimeOffset.Now,
            operation,
            enrollmentId));
    }

    /// <summary>Скидає буфер і закриває файл. Повторний виклик нічого не робить.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
