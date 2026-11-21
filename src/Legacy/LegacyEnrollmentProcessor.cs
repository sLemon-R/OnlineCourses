using System;
using System.Collections.Generic;

namespace OnlineCourses.Legacy;

public enum ClientKind { Regular, Vip, Staff }

public enum DocState { Draft, New, Paid, Closed }

public sealed class Learner
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
    public ClientKind Kind { get; init; }
    public int DoneCount { get; init; }
    public DateTime SinceUtc { get; init; }

    public bool HasValidEmail =>
        Email is not null && Email.Contains("@");

    public bool IsLoyal =>
        DoneCount > PricingRules.LoyalOrders;
}

public class CourseLine
{
    public string Code = "";
    public int Seats;
    public decimal Price;
    public string Group = "";
}

public class LegacyEnrollmentProcessor
{
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly DiscountPolicy _discounts = new();
    private readonly ShippingPolicy _shipping = new();
    private readonly List<string> _log = new();

    public string Handle(int docId, int clientId,
        string clientName, string? clientMail,
        ClientKind clientKind, int clientDone,
        List<CourseLine>? items, DocState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        var error = Validate(items, state, clientMail);
        if (error != null) return error;
        var lines = items!;
        _log.Add("ok " + docId);

        _tmpSum = Subtotal(lines);
        _tmpDiscount = _discounts.For(_tmpSum, clientKind, clientDone);
        decimal ship = _shipping.For(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = "Документ #" + docId + "\n";
        txt += "Клієнт: " + clientName + "\n";
        for (int i = 0; i < lines.Count; i++)
        {
            txt += lines[i].Code + " x" + lines[i].Seats + " = " +
                (lines[i].Seats * lines[i].Price)
                .ToString("0.00") + " " + currency + "\n";
        }

        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
            " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + currency + "\n";
        if (sendMail)
        {
            _log.Add("mail -> " + clientMail);
        }

        return txt;
    }

    private static string? Validate(List<CourseLine>? items,
        DocState state, string? clientMail)
    {
        if (items is null) return "ERR: null";
        if (items.Count == 0) return "ERR: empty";
        if (state is not (DocState.New or DocState.Paid))
            return "ERR: state";
        if (!IsValidMail(clientMail)) return "ERR: mail";
        return null;
    }

    private static bool IsValidMail(string? mail) =>
        mail != null && mail.Contains("@");

    private static decimal Subtotal(IReadOnlyList<CourseLine> items)
    {
        decimal sum = 0m;
        foreach (var it in items) sum += it.Seats * it.Price;
        return sum;
    }

    public decimal Preview(ClientKind clientKind, int clientDone,
        List<CourseLine> items)
    {
        var subtotal = Subtotal(items);
        var discount = _discounts.For(subtotal, clientKind, clientDone);
        var payable = subtotal - discount;
        return payable + _shipping.For(payable);
    }

    public string DescribeClient(Learner c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == ClientKind.Vip) s += " [VIP]";
        if (c.IsLoyal) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
