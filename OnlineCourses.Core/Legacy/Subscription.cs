using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineCourses.Core.Legacy;

/// <summary>
/// Абонемент слухача: набір курсів, на які він записався, з розрахунком вартості.
/// </summary>
public class Subscription
{
    private readonly List<string[]> _lines = new List<string[]>();

    // public string prim; // примітка, поки не треба

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

    /// <summary>Отримує стан абонемента: 0 — нова, 1 — оплачена, 2 — навчання, 3 — скасована.</summary>
    public int Status { get; private set; }

    /// <summary>Отримує дату й час створення абонемента.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає абонемент у списку за номером.
    /// </summary>
    /// <param name="subscriptions">Список абонементів.</param>
    /// <param name="subscriptionId">Номер, який шукаємо.</param>
    /// <returns>Знайдений абонемент або null, якщо такого номера немає.</returns>
    public static Subscription FindById(List<Subscription> subscriptions, string subscriptionId)
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
        line[1] = seats.ToString();
        line[2] = unitPrice.ToString();
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
            int seats = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            total = total + seats * unitPrice;
            lineCount = lineCount + 1;
        }

        // if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка

        // знижки не сумуються: постійному слухачу дається тільки його знижка
        if (isReturningListener == true && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else
        {
            total = total;
        }

        if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму вже після всіх знижок
        total = total + total * 0.2m;
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан абонемента, якщо перехід дозволений правилами предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан абонемента.</param>
    /// <returns>true, якщо перехід виконано; false, якщо він заборонений.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
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
        if (Id != null && Id != ""
            && ListenerName != null
            && ListenerName.Length > 2
            && _lines.Count > 0
            && _lines.Count < 100
            && Status >= 0
            && Status <= 3)
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
        string report = "";
        for (int i = 0; i < _lines.Count; i++)
        {
            int seats = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            report = report + "Курс: " + _lines[i][0]
                + "; місць: " + _lines[i][1]
                + "; ціна: " + _lines[i][2]
                + "; сума: " + (seats * unitPrice) + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }
}
