namespace OnlineCourses.Legacy;

public sealed class DiscountPolicy
{
    public decimal For(decimal subtotal, ClientKind kind, int doneCount)
    {
        var rate = RateFor(kind, doneCount);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private static decimal RateFor(ClientKind kind, int doneCount) => kind switch
    {
        ClientKind.Vip => PricingRules.VipRate,
        ClientKind.Staff => PricingRules.StaffRate,
        _ => doneCount > PricingRules.LoyalOrders ? PricingRules.LoyalRate : 0m
    };
}

public sealed class ShippingPolicy
{
    public decimal For(decimal payable) =>
        payable < PricingRules.FreeShipFrom
            ? PricingRules.ShipCost : 0m;
}
