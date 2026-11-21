using System.Globalization;

namespace OnlineCourses.Core.Legacy;

/// <summary>
/// Абонемент слухача: набір курсів, на які він записався, з розрахунком вартості.
/// </summary>
public class Subscription
{
    /// <summary>Стан «нова».</summary>
    public const int StatusNew = 0;

    /// <summary>Стан «оплачена».</summary>
    public const int StatusPaid = 1;

    /// <summary>Стан «навчання триває».</summary>
    public const int StatusInProgress = 2;

    /// <summary>Стан «скасована».</summary>
    public const int StatusCancelled = 3;

    /// <summary>Ставка податку на додану вартість.</summary>
    private const decimal VatRate = 0.2m;

    /// <summary>Поріг суми для знижки постійному слухачу.</summary>
    private const decimal ReturningDiscountThreshold = 1000m;

    /// <summary>Частка знижки постійному слухачу.</summary>
    private const decimal ReturningDiscountRate = 0.10m;

    /// <summary>Поріг суми для знижки на великий абонемент.</summary>
    private const decimal LargeSubscriptionThreshold = 5000m;

    /// <summary>Частка знижки на великий абонемент.</summary>
    private const decimal LargeSubscriptionDiscountRate = 0.15m;

    /// <summary>Кількість рядків, з якої діє знижка за кількість курсів.</summary>
    private const int BulkLineCount = 10;

    /// <summary>Сума знижки за кількість курсів, грн.</summary>
    private const decimal BulkDiscountAmount = 100m;

    /// <summary>Найбільша допустима кількість рядків в абонементі.</summary>
    private const int MaxLineCount = 100;

    /// <summary>Найкоротше допустиме прізвище слухача.</summary>
    private const int MinListenerNameLength = 3;

    private readonly List<string[]> _lines = new List<string[]>();

    /// <summary>
    /// Створює новий абонемент у стані «нова».
    /// </summary>
    /// <param name="id">Номер абонемента.</param>
    /// <param name="listenerName">Прізвище слухача.</param>
    public Subscription(string id, string listenerName)
    {
        Id = id;
        ListenerName = listenerName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Отримує номер абонемента.</summary>
    public string Id { get; private set; }

    /// <summary>Отримує прізвище слухача.</summary>
    public string ListenerName { get; private set; }

    /// <summary>Отримує стан абонемента (одна з констант Status...).</summary>
    public int Status { get; private set; }

    /// <summary>Отримує дату й час створення абонемента.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає абонемент у списку за номером.
    /// </summary>
    /// <param name="subscriptions">Список абонементів.</param>
    /// <param name="subscriptionId">Номер, який шукаємо.</param>
    /// <returns>Знайдений абонемент або null, якщо такого номера немає.</returns>
    public static Subscription? FindById(List<Subscription> subscriptions, string subscriptionId)
    {
        for (int i = 0; i < subscriptions.Count; i++)
        {
            if (subscriptions[i].Id == subscriptionId)
            {
                return subscriptions[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає до абонемента рядок із курсом.
    /// </summary>
    /// <param name="courseCode">Код курсу.</param>
    /// <param name="seats">Кількість місць.</param>
    /// <param name="unitPrice">Ціна за одне місце, грн.</param>
    public void AddLine(string courseCode, int seats, decimal unitPrice)
    {
        string[] line = new string[3];
        line[0] = courseCode;
        line[1] = seats.ToString(CultureInfo.CurrentCulture);
        line[2] = unitPrice.ToString(CultureInfo.CurrentCulture);
        _lines.Add(line);
    }

    /// <summary>
    /// Рахує вартість абонемента зі знижками та ПДВ.
    /// </summary>
    /// <param name="isReturningListener">Чи слухач уже навчався на платформі.</param>
    /// <returns>Вартість до сплати, округлена до копійок.</returns>
    public decimal CalculateTotal(bool isReturningListener)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int seats = int.Parse(_lines[i][1], CultureInfo.CurrentCulture);
            decimal unitPrice = decimal.Parse(_lines[i][2], CultureInfo.CurrentCulture);
            total = total + (seats * unitPrice);
            lineCount = lineCount + 1;
        }

        // знижки не сумуються: постійному слухачу дається тільки його знижка
        if (isReturningListener && total > ReturningDiscountThreshold)
        {
            total *= 1m - ReturningDiscountRate;
        }
        else if (total > LargeSubscriptionThreshold)
        {
            total *= 1m - LargeSubscriptionDiscountRate;
        }

        if (lineCount > BulkLineCount)
        {
            total -= BulkDiscountAmount;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму вже після всіх знижок
        total += total * VatRate;
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан абонемента, якщо перехід дозволений правилами предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан абонемента.</param>
    /// <returns>true, якщо перехід виконано; false, якщо він заборонений.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == StatusNew && newStatus == StatusPaid)
        {
            Status = StatusPaid;
            return true;
        }

        if (Status == StatusPaid && newStatus == StatusInProgress)
        {
            Status = StatusInProgress;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє, чи абонемент заповнений коректно.
    /// </summary>
    /// <returns>true, якщо номер, слухач, рядки й стан коректні.</returns>
    public bool IsValid()
    {
        if (!string.IsNullOrEmpty(Id)
            && ListenerName != null
            && ListenerName.Length > MinListenerNameLength - 1
            && _lines.Count > 0
            && _lines.Count < MaxLineCount
            && Status >= StatusNew
            && Status <= StatusCancelled)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Формує текстовий звіт по рядках абонемента.
    /// </summary>
    /// <returns>Звіт: по рядку на курс і підсумок.</returns>
    public string BuildReport()
    {
        string report = string.Empty;
        for (int i = 0; i < _lines.Count; i++)
        {
            int seats = int.Parse(_lines[i][1], CultureInfo.CurrentCulture);
            decimal unitPrice = decimal.Parse(_lines[i][2], CultureInfo.CurrentCulture);
            report = report + "Курс: " + _lines[i][0]
                + "; місць: " + _lines[i][1]
                + "; ціна: " + _lines[i][2]
                + "; сума: " + (seats * unitPrice) + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }
}
