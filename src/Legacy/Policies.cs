namespace OnlineCourses.Legacy;

public sealed class DiscountPolicy
{
    public decimal For(decimal subtotal, Learner client)
    {
        var rate = RateFor(client);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private static decimal RateFor(Learner client) => client.Kind switch
    {
        ClientKind.Vip => PricingRules.VipRate,
        ClientKind.Staff => PricingRules.StaffRate,
        _ => client.IsLoyal ? PricingRules.LoyalRate : 0m
    };
}

public sealed class ShippingPolicy
{
    public decimal For(decimal payable) =>
        payable < PricingRules.FreeShipFrom
            ? PricingRules.ShipCost : 0m;
}
