using OnlineCourses.Core.Pricing;

namespace OnlineCourses.Tests;

public class EnrollmentDiscountTests
{
    [Fact]
    public void For_NegativeAmount_ThrowsOutOfRange()
    {
        decimal amount = -0.01m;

        void Act() => EnrollmentDiscount.For(amount, true);

        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void For_ZeroAmountNewListener_ReturnsZero()
    {
        decimal amount = 0m;

        decimal actual = EnrollmentDiscount.For(amount, false);

        Assert.Equal(0m, actual);
    }

    [Fact]
    public void For_SmallEnrollmentReturningListener_ReturnsFivePercent()
    {
        decimal amount = 400m;

        decimal actual = EnrollmentDiscount.For(amount, true);

        Assert.Equal(20m, actual);
    }

    [Fact]
    public void For_JustBelowBigEnrollment_ReturnsZero()
    {
        decimal amount = 999.99m;

        decimal actual = EnrollmentDiscount.For(amount, false);

        Assert.Equal(0m, actual);
    }

    [Fact]
    public void For_ExactlyBigEnrollment_ReturnsTenPercent()
    {
        decimal amount = 1000m;

        decimal actual = EnrollmentDiscount.For(amount, false);

        Assert.Equal(100m, actual);
    }

    [Fact]
    public void For_BigEnrollmentReturningListener_ReturnsFifteenPercent()
    {
        decimal amount = 1000m;

        decimal actual = EnrollmentDiscount.For(amount, true);

        Assert.Equal(150m, actual);
    }
}
