using OnlineCourses.Core.Pricing;

namespace OnlineCourses.Tests;

public class PackageDiscountTests
{
    [Theory]
    [InlineData(1, false, 0)]
    [InlineData(1, true, 0.03)]
    [InlineData(2, false, 0.05)]
    [InlineData(2, true, 0.08)]
    [InlineData(3, false, 0.10)]
    [InlineData(3, true, 0.13)]
    [InlineData(7, false, 0.10)]
    [InlineData(7, true, 0.13)]
    public void RateFor_DecisionTable_MatchesSpec(int courses, bool isReturning, double expected)
    {
        decimal actual = PackageDiscount.RateFor(courses, isReturning);

        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RateFor_NoCourses_ThrowsOutOfRange(int courses)
    {
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => PackageDiscount.RateFor(courses, false));

        Assert.Equal("courseCount", ex.ParamName);
    }

    [Fact]
    public void DiscountFor_ZeroSubtotal_ReturnsZero()
    {
        decimal actual = PackageDiscount.DiscountFor(0m, 3, true);

        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_NegativeSubtotal_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PackageDiscount.DiscountFor(-0.01m, 1, false));
    }

    [Fact]
    public void DiscountFor_RoundsHalfAwayFromZero()
    {
        decimal actual = PackageDiscount.DiscountFor(100.10m, 2, false);

        Assert.Equal(5.01m, actual);
    }

    [Fact]
    public void DiscountFor_HugePackage_IsCappedAtMax()
    {
        decimal actual = PackageDiscount.DiscountFor(20000m, 3, true);

        Assert.Equal(PackageDiscount.MaxDiscount, actual);
    }
}
