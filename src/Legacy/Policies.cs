namespace OnlineCourses.Legacy;

public interface IDiscountRule
{
    decimal? RateFor(Learner client);
}

public sealed class VipDiscountRule : IDiscountRule
{
    public decimal? RateFor(Learner client) =>
        client.Kind == ClientKind.Vip ? PricingRules.VipRate : null;
}

public sealed class StaffDiscountRule : IDiscountRule
{
    public decimal? RateFor(Learner client) =>
        client.Kind == ClientKind.Staff ? PricingRules.StaffRate : null;
}

public sealed class LoyalDiscountRule : IDiscountRule
{
    public decimal? RateFor(Learner client) =>
        client.Kind == ClientKind.Regular && client.IsLoyal
            ? PricingRules.LoyalRate : null;
}

public sealed class DiscountPolicy
{
    private readonly IReadOnlyList<IDiscountRule> _rules = new IDiscountRule[]
    {
        new VipDiscountRule(),
        new StaffDiscountRule(),
        new LoyalDiscountRule()
    };

    public decimal For(decimal subtotal, Learner client)
    {
        var rate = RateFor(client);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private decimal RateFor(Learner client)
    {
        foreach (var rule in _rules)
        {
            var rate = rule.RateFor(client);
            if (rate is not null) return rate.Value;
        }

        return 0m;
    }
}

public sealed class ShippingPolicy
{
    public decimal For(decimal payable) =>
        payable < PricingRules.FreeShipFrom
            ? PricingRules.ShipCost : 0m;
}
