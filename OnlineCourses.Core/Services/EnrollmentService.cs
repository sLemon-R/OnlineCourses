using System.Globalization;
using Microsoft.Extensions.Logging;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

namespace OnlineCourses.Core.Services;

/// <summary>Прикладний сервіс: оформлення заявки за обраною політикою ціни.</summary>
public sealed partial class EnrollmentService
{
    private static readonly Func<ILogger, int, IDisposable?> EnrollmentScope =
        LoggerMessage.DefineScope<int>("Enrollment:{EnrollmentId}");

    private readonly IEnrollmentRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly ILogger<EnrollmentService> _log;

    /// <summary>Створює сервіс. Залежності передаються ззовні.</summary>
    /// <param name="repository">Сховище заявок.</param>
    /// <param name="pricing">Політика ціни.</param>
    /// <param name="log">Журнал сервісу.</param>
    public EnrollmentService(
        IEnrollmentRepository repository,
        IPricingPolicy pricing,
        ILogger<EnrollmentService> log)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        ArgumentNullException.ThrowIfNull(log);
        _repository = repository;
        _pricing = pricing;
        _log = log;
    }

    /// <summary>Рахує вартість заявки за політикою. Нічого не змінює.</summary>
    /// <param name="enrollment">Заявка.</param>
    /// <returns>Сума позицій за політикою, грн.</returns>
    public decimal TotalOf(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        decimal total = 0m;
        foreach (EnrollmentLine line in enrollment.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return total;
    }

    /// <summary>Додає позицію до заявки. Приймає вже перевірений об'єкт, а не рядки.</summary>
    /// <param name="enrollmentId">Номер заявки.</param>
    /// <param name="line">Позиція заявки.</param>
    /// <exception cref="DomainRuleException">Заявку не знайдено.</exception>
    public void AddLine(int enrollmentId, EnrollmentLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        Enrollment enrollment = Find(enrollmentId);
        enrollment.AddLine(line);
        _repository.Add(enrollment);
    }

    /// <summary>Оплачує заявку: переводить у Paid і зберігає.</summary>
    /// <param name="enrollmentId">Номер заявки.</param>
    /// <param name="amount">Сплачена сума, грн.</param>
    /// <exception cref="DomainRuleException">
    /// Заявку не знайдено або сума менша за вартість.
    /// </exception>
    public void Pay(int enrollmentId, decimal amount)
    {
        using IDisposable? scope = EnrollmentScope(_log, enrollmentId);
        LogPaymentStarted(amount);
        Enrollment enrollment = Find(enrollmentId);
        decimal total = TotalOf(enrollment);
        if (amount > total)
        {
            LogOverpayment(amount - total);
        }

        if (amount < total)
        {
            LogUnderpayment(amount, total);
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "Сума {0:F2} менша за вартість заявки {1:F2}",
                amount,
                total);
            throw new DomainRuleException("enrollment.amount", message);
        }

        enrollment.ChangeStatus(EnrollmentStatus.Paid);
        _repository.Add(enrollment);
        LogPaid(amount);
    }

    /// <summary>Шукає заявку. null тут — звичайна відповідь «не знайдено», а не помилка.</summary>
    /// <param name="enrollmentId">Номер заявки.</param>
    /// <returns>Заявка або null.</returns>
    public Enrollment? FindOrDefault(int enrollmentId) => _repository.GetById(enrollmentId);

    /// <summary>Скасовує заявку. Винятки не перехоплюються: їх бачить той, хто викликав.</summary>
    /// <param name="enrollmentId">Номер заявки.</param>
    /// <param name="reason">Причина скасування.</param>
    /// <exception cref="DomainRuleException">Заявку не знайдено.</exception>
    /// <exception cref="InvalidOperationException">Скасування заборонене.</exception>
    public void Cancel(int enrollmentId, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        Enrollment enrollment = Find(enrollmentId);
        enrollment.ChangeStatus(EnrollmentStatus.Cancelled);
        _repository.Add(enrollment);
    }

    private Enrollment Find(int enrollmentId) =>
        _repository.GetById(enrollmentId)
            ?? throw new DomainRuleException(
                "enrollment.exists",
                "Заявку " + enrollmentId.ToString(CultureInfo.InvariantCulture) + " не знайдено");

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Початок оплати, сума {Amount}")]
    private partial void LogPaymentStarted(decimal amount);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Оплачено на {Amount}")]
    private partial void LogPaid(decimal amount);

    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Переплата {Extra}")]
    private partial void LogOverpayment(decimal extra);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "Сума {Amount} менша за вартість {Total}, оплату відхилено")]
    private partial void LogUnderpayment(decimal amount, decimal total);
}
