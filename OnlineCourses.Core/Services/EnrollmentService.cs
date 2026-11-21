using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

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

    /// <summary>Оформлює заявку: рахує вартість, переводить у Paid і зберігає.</summary>
    /// <param name="enrollment">Нова заявка з позиціями.</param>
    /// <returns>Вартість заявки, грн.</returns>
    public decimal Place(Enrollment enrollment)
    {
        decimal total = TotalOf(enrollment);
        enrollment.ChangeStatus(EnrollmentStatus.Paid);
        _repository.Add(enrollment);
        return total;
    }
}
