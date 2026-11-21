namespace OnlineCourses.Core.Errors;

/// <summary>Порушено правило предметної області.</summary>
public class DomainRuleException : Exception
{
    /// <summary>Створює виняток без опису.</summary>
    public DomainRuleException()
        : this("unknown", "Порушено правило предметної області.")
    {
    }

    /// <summary>Створює виняток з повідомленням.</summary>
    /// <param name="message">Повідомлення українською.</param>
    public DomainRuleException(string message)
        : this("unknown", message)
    {
    }

    /// <summary>Створює виняток з повідомленням і внутрішнім винятком.</summary>
    /// <param name="message">Повідомлення українською.</param>
    /// <param name="innerException">Причина.</param>
    public DomainRuleException(string message, Exception innerException)
        : this("unknown", message, innerException)
    {
    }

    /// <summary>Створює виняток для конкретного правила.</summary>
    /// <param name="rule">Ідентифікатор правила, напр. enrollment.exists.</param>
    /// <param name="message">Повідомлення українською.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>Створює виняток для правила з внутрішнім винятком.</summary>
    /// <param name="rule">Ідентифікатор правила.</param>
    /// <param name="message">Повідомлення українською.</param>
    /// <param name="innerException">Причина.</param>
    public DomainRuleException(string rule, string message, Exception innerException)
        : base(message, innerException)
    {
        Rule = rule;
    }

    /// <summary>Отримує ідентифікатор порушеного правила.</summary>
    public string Rule { get; } = "unknown";
}
