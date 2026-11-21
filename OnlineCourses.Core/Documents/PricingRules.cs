namespace OnlineCourses.Core.Documents;

/// <summary>Правила ціноутворення заявки на навчання.</summary>
public static class PricingRules
{
    /// <summary>Кількість місць у рядку, з якої діє знижка за групу.</summary>
    public const int BulkSeats = 10;

    /// <summary>Частка знижки за групу.</summary>
    public const decimal BulkRate = 0.05m;

    /// <summary>Частка знижки постійному слухачу.</summary>
    public const decimal ReturningListenerRate = 0.07m;

    /// <summary>Сума, понад яку діє велика знижка за обсяг.</summary>
    public const decimal LargeEnrollmentFrom = 5000m;

    /// <summary>Частка великої знижки за обсяг.</summary>
    public const decimal LargeEnrollmentRate = 0.10m;

    /// <summary>Сума, понад яку діє середня знижка за обсяг.</summary>
    public const decimal MediumEnrollmentFrom = 2000m;

    /// <summary>Частка середньої знижки за обсяг.</summary>
    public const decimal MediumEnrollmentRate = 0.05m;

    /// <summary>Частка знижки за заявку, створену в неділю.</summary>
    public const decimal WeekendRate = 0.02m;

    /// <summary>Частка знижки за купоном SALE10.</summary>
    public const decimal CouponSaleRate = 0.10m;

    /// <summary>Фіксована знижка за купоном MINUS200, грн.</summary>
    public const decimal CouponFixed = 200m;

    /// <summary>Сума, понад яку діє купон MINUS200.</summary>
    public const decimal CouponFixedFrom = 1000m;

    /// <summary>Сума, від якої доступ до матеріалів надсилається безкоштовно.</summary>
    public const decimal FreeDeliveryFrom = 1000m;

    /// <summary>Код купона з відсотковою знижкою.</summary>
    public const string SaleCoupon = "SALE10";

    /// <summary>Код купона з фіксованою знижкою.</summary>
    public const string FixedCoupon = "MINUS200";
}
