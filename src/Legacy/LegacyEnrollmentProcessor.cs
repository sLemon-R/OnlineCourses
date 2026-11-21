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
    private readonly ReportBuilder _report = new();
    private readonly List<string> _log = new();

    public string Handle(EnrollmentRequest req)
    {
        var error = Validate(req);
        if (error != null) return error;
        _log.Add("ok " + req.DocId);

        _tmpSum = Subtotal(req.Items!);
        _tmpDiscount = _discounts.For(_tmpSum, req.Client);
        decimal ship = _shipping.For(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Client.Email);
        }

        return _report.Build(req, _tmpDiscount, ship, total);
    }

    private static string? Validate(EnrollmentRequest req)
    {
        if (req.Items is null) return "ERR: null";
        if (req.Items.Count == 0) return "ERR: empty";
        if (req.State is not (DocState.New or DocState.Paid))
            return "ERR: state";
        if (!req.Client.HasValidEmail) return "ERR: mail";
        return null;
    }

    private static decimal Subtotal(IReadOnlyList<CourseLine> items)
    {
        decimal sum = 0m;
        foreach (var it in items) sum += it.Seats * it.Price;
        return sum;
    }

    public decimal Preview(Learner client, IReadOnlyList<CourseLine> items)
    {
        var subtotal = Subtotal(items);
        var discount = _discounts.For(subtotal, client);
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
