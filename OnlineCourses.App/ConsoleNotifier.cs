using OnlineCourses.Core.Abstractions;

namespace OnlineCourses.App;

/// <summary>Повідомлення виводяться в консоль замість пошти.</summary>
internal sealed class ConsoleNotifier : INotifier
{
    /// <inheritdoc/>
    public void Notify(string recipient, string subject, string body)
    {
        Console.WriteLine("[лист для " + recipient + "] " + subject + ": " + body);
    }
}
