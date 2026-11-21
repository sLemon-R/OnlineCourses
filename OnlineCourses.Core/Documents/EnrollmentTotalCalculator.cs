using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Documents;

/// <summary>Розрахунок вартості заявки на навчання.</summary>
public static class EnrollmentTotalCalculator
{
    /// <summary>Рахує вартість заявки, наступний стан і за потреби друкує їх.</summary>
    /// <param name="enrollmentId">Номер заявки.</param>
    /// <param name="listenerName">Прізвище слухача.</param>
    /// <param name="listenerEmail">Пошта слухача.</param>
    /// <param name="isReturning">Чи слухач постійний.</param>
    /// <param name="lines">Позиції заявки.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <param name="couponCode">Код купона або порожній рядок.</param>
    /// <param name="status">Поточний стан заявки.</param>
    /// <param name="printToConsole">Чи друкувати результат.</param>
    /// <param name="deliveryPrice">Ціна надсилання матеріалів, грн.</param>
    /// <param name="newStatus">Наступний стан заявки.</param>
    /// <returns>Вартість заявки, округлена до копійок.</returns>
    public static decimal Process(
        int enrollmentId,
        string listenerName,
        string listenerEmail,
        bool isReturning,
        IReadOnlyList<CourseItem> lines,
        DateOnly createdAt,
        string couponCode,
        EnrollmentStatus status,
        bool printToConsole,
        decimal deliveryPrice,
        out EnrollmentStatus newStatus)
    {
        EnrollmentGuards.EnsureHeaderValid(enrollmentId, listenerName, listenerEmail);
        decimal total = SubtotalOf(lines);
        total -= ListenerDiscountOf(total, isReturning);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, couponCode);
        total -= WeekendDiscountOf(total, createdAt);
        total += DeliveryFeeOf(total, deliveryPrice);
        total = status == EnrollmentStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), 2);
        newStatus = NextStatusOf(status, total);
        if (printToConsole)
        {
            Console.WriteLine("Заявка № " + enrollmentId);
            Console.WriteLine("Слухач: " + listenerName);
            foreach (CourseItem line in lines)
            {
                Console.WriteLine(line.CourseCode + " x " + line.Seats);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
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
