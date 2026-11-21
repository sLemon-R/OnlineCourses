namespace OnlineCourses.Core.Domain;

/// <summary>Стан заявки на навчання в її життєвому циклі.</summary>
public enum EnrollmentStatus
{
    /// <summary>Нова заявка, ще не оплачена.</summary>
    New = 0,

    /// <summary>Заявку оплачено.</summary>
    Paid = 1,

    /// <summary>Слухач проходить навчання.</summary>
    InProgress = 2,

    /// <summary>Навчання завершено.</summary>
    Completed = 3,

    /// <summary>Заявку скасовано.</summary>
    Cancelled = 4,
}
