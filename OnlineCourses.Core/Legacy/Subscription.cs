using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineCourses.Core.Legacy;

// клас
public class Subscription
{
    private readonly List<string[]> _lines = new List<string[]>();

    // public string prim; // примітка, поки не треба

    // конструктор
    public Subscription(string id, string listenerName)
    {
        Id = id; // ставимо id
        ListenerName = listenerName; // ставимо sl
        CreatedAt = DateTime.Now; // ставимо дату
    }

    public string Id { get; private set; }

    public string ListenerName { get; private set; }

    public int Status { get; private set; } // 0-нова,1-оплач,2-навч,3-скасов

    public DateTime CreatedAt { get; private set; }

    // пошук
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

    // метод додавання
    public void AddLine(string courseCode, int seats, decimal unitPrice)
    {
        string[] line = new string[3];
        line[0] = courseCode;
        line[1] = seats.ToString();
        line[2] = unitPrice.ToString();
        _lines.Add(line); // додаємо t у ln
    }

    // ProcessData
    public decimal CalculateTotal(bool isReturningListener)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int seats = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            total = total + seats * unitPrice; // додаємо до суми
            lineCount = lineCount + 1; // збільшуємо kolvo на одиницю
        }

        // if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка
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

        total = total + total * 0.2m;
        return Math.Round(total, 2); // повертаємо sum1
    }

    // міняємо статус
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

        return false; // не можна
    }

    // перевірка
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

    // звіт
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
        return report; // повертаємо s
    }
}
