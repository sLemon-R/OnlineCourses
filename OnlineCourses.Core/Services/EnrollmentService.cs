using System.Globalization;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

namespace OnlineCourses.Core.Services;

/// <summary>Прикладний сервіс: оформлення заявки за обраною політикою ціни.</summary>
public sealed class EnrollmentService
{
    private readonly IEnrollmentRepository _repository;
    private readonly IPricingPolicy _pricing;

    /// <summary>Створює сервіс. Залежності передаються ззовні.</summary>
    /// <param name="repository">Сховище заявок.</param>
    /// <param name="pricing">Політика ціни.</param>
    public EnrollmentService(IEnrollmentRepository repository, IPricingPolicy pricing)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        _repository = repository;
        _pricing = pricing;
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
        Enrollment enrollment = Find(enrollmentId);
        decimal total = TotalOf(enrollment);
        if (amount < total)
        {
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "Сума {0:F2} менша за вартість заявки {1:F2}",
                amount,
                total);
            throw new DomainRuleException("enrollment.amount", message);
        }

        enrollment.ChangeStatus(EnrollmentStatus.Paid);
        _repository.Add(enrollment);
    }

    private Enrollment Find(int enrollmentId) =>
        _repository.GetById(enrollmentId)
            ?? throw new DomainRuleException(
                "enrollment.exists",
                "Заявку " + enrollmentId.ToString(CultureInfo.InvariantCulture) + " не знайдено");
}
