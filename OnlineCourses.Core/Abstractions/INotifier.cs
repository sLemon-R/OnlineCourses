namespace OnlineCourses.Core.Abstractions;

/// <summary>Надсилання повідомлень слухачу.</summary>
public interface INotifier
{
    /// <summary>Надсилає повідомлення.</summary>
    /// <param name="recipient">Адреса отримувача.</param>
    /// <param name="subject">Тема.</param>
    /// <param name="body">Текст.</param>
    void Notify(string recipient, string subject, string body);
}
