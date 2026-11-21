using Microsoft.Extensions.Logging;

namespace OnlineCourses.App.Logging;

/// <summary>Постачальник журналу, що дописує рядки у текстовий файл.</summary>
internal sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly StreamWriter _writer;
    private readonly object _gate = new();

    /// <summary>Відкриває файл журналу для дописування.</summary>
    /// <param name="path">Шлях до файлу; тека створюється, якщо її немає.</param>
    public FileLoggerProvider(string path)
    {
        string? folder = Path.GetDirectoryName(Path.GetFullPath(path));
        if (folder is not null)
        {
            Directory.CreateDirectory(folder);
        }

        _writer = new StreamWriter(path, append: true) { AutoFlush = true };
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, _writer, _gate);

    /// <inheritdoc/>
    public void Dispose() => _writer.Dispose();
}
