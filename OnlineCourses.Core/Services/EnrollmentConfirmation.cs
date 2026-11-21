using System.Globalization;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Services;

/// <summary>Підтвердження оформленої заявки слухачу.</summary>
public sealed class EnrollmentConfirmation
{
    private readonly INotifier _notifier;

    /// <summary>Створює сервіс підтвердження.</summary>
    /// <param name="notifier">Спосіб надсилання повідомлень.</param>
    public EnrollmentConfirmation(INotifier notifier)
    {
        ArgumentNullException.ThrowIfNull(notifier);
        _notifier = notifier;
    }

    /// <summary>Надсилає слухачу підтвердження з сумою.</summary>
    /// <param name="listener">Слухач.</param>
    /// <param name="enrollment">Оформлена заявка.</param>
    /// <param name="total">Вартість, грн.</param>
    public void Send(Listener listener, Enrollment enrollment, decimal total)
    {
        ArgumentNullException.ThrowIfNull(listener);
        ArgumentNullException.ThrowIfNull(enrollment);
        string subject = string.Format(CultureInfo.InvariantCulture, "Заявка № {0}", enrollment.Id);
        string body = string.Format(CultureInfo.InvariantCulture, "До сплати: {0:F2} грн", total);
        _notifier.Notify(listener.Email, subject, body);
    }
}
