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

    public string Describe()
    {
        var s = Name.Trim().ToUpper();
        if (Kind == ClientKind.Vip) s += " [VIP]";
        if (IsLoyal) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + Email.ToLower() + ">";
        return s + " стаж " + (DateTime.Now.Year - SinceUtc.Year);
    }
}

public class CourseLine
{
    public string Code = "";
    public int Seats;
    public decimal Price;
    public string Group = "";
}

public sealed class EnrollmentProcessor
{
    private readonly DiscountPolicy _discounts = new();
    private readonly ShippingPolicy _shipping = new();
    private readonly ReportBuilder _report = new();
    private readonly List<string> _log = new();

    public string Handle(EnrollmentRequest req)
    {
        var error = Validate(req);
        if (error != null) return error;
        _log.Add("ok " + req.DocId);

        var subtotal = Subtotal(req.Items!);
        var discount = _discounts.For(subtotal, req.Client);
        var shipping = _shipping.For(subtotal - discount);
        var total = subtotal - discount + shipping;

        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Client.Email);
        }

        return _report.Build(req, discount, shipping, total);
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

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
