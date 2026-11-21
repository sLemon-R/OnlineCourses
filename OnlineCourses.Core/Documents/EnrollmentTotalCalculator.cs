using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Documents;

/// <summary>Розрахунок вартості заявки на навчання.</summary>
public static class EnrollmentTotalCalculator
{
    /// <summary>Рахує вартість заявки й наступний стан. Нічого не друкує й не змінює.</summary>
    /// <param name="request">Запит на розрахунок.</param>
    /// <returns>Вартість, округлена до копійок, і наступний стан.</returns>
    public static TotalResult Calculate(TotalRequest request)
    {
        EnrollmentGuards.EnsureRequestValid(request);
        decimal total = SubtotalOf(request.Lines);
        total -= ListenerDiscountOf(total, request.Listener.IsReturning);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, request.CouponCode);
        total -= WeekendDiscountOf(total, request.CreatedAt);
        total += DeliveryFeeOf(total, request.DeliveryPrice);
        total = request.Status == EnrollmentStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), 2);
        return new TotalResult { Total = total, Status = NextStatusOf(request.Status, total) };
    }

    /// <summary>Сума позицій зі знижкою за групу.</summary>
    /// <param name="lines">Позиції заявки.</param>
    /// <returns>Сума позицій, грн.</returns>
    public static decimal SubtotalOf(IReadOnlyList<CourseItem> lines)
    {
        EnrollmentGuards.EnsureLinesValid(lines);
        decimal subtotal = 0m;
        foreach (CourseItem line in lines)
        {
            decimal sum = line.Seats * line.UnitPrice;
            if (line.Seats >= PricingRules.BulkSeats)
            {
                sum -= sum * PricingRules.BulkRate;
            }

            subtotal += sum;
        }

        return subtotal;
    }

    /// <summary>Знижка постійному слухачу.</summary>
    /// <param name="amount">Сума до знижки.</param>
    /// <param name="isReturning">Чи слухач постійний.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal ListenerDiscountOf(decimal amount, bool isReturning)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isReturning ? amount * PricingRules.ReturningListenerRate : 0m;
    }

    /// <summary>Знижка за обсяг заявки.</summary>
    /// <param name="amount">Сума до знижки.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (amount > PricingRules.LargeEnrollmentFrom)
        {
            return amount * PricingRules.LargeEnrollmentRate;
        }

        if (amount > PricingRules.MediumEnrollmentFrom)
        {
            return amount * PricingRules.MediumEnrollmentRate;
        }

        return 0m;
    }

    /// <summary>Знижка за кодом купона.</summary>
    /// <param name="amount">Сума до знижки.</param>
    /// <param name="couponCode">Код купона або null.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal CouponDiscountOf(decimal amount, string? couponCode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return couponCode switch
        {
            PricingRules.SaleCoupon => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon when amount > PricingRules.CouponFixedFrom
                => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    /// <summary>Знижка за заявку, створену в неділю.</summary>
    /// <param name="amount">Сума до знижки.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal WeekendDiscountOf(decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday ? amount * PricingRules.WeekendRate : 0m;
    }

    /// <summary>Плата за надсилання матеріалів.</summary>
    /// <param name="amount">Сума після знижок.</param>
    /// <param name="deliveryPrice">Ціна надсилання, грн.</param>
    /// <returns>Плата, грн; 0, якщо сума від 1000 грн.</returns>
    public static decimal DeliveryFeeOf(decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom ? deliveryPrice : 0m;
    }

    /// <summary>Наступний стан заявки після розрахунку. Нічого не змінює.</summary>
    /// <param name="current">Поточний стан.</param>
    /// <param name="total">Розрахована вартість.</param>
    /// <returns>Наступний стан.</returns>
    public static EnrollmentStatus NextStatusOf(EnrollmentStatus current, decimal total) =>
        current switch
        {
            EnrollmentStatus.New when total > 0m => EnrollmentStatus.Paid,
            EnrollmentStatus.Paid => EnrollmentStatus.InProgress,
            _ => current,
        };
}
