using OnlineCourses.Core.Pricing;

namespace OnlineCourses.Tests;

public class EnrollmentDiscountTests
{
    [Theory]
    [InlineData(0, false, 0)]
    [InlineData(400, true, 20)]
    [InlineData(999.99, false, 0)]
    [InlineData(999.99, true, 50)]
    [InlineData(1000, false, 100)]
    [InlineData(1000, true, 150)]
    [InlineData(1333.26, true, 199.99)]
    [InlineData(1333.34, true, 200)]
    public void For_Boundaries_MatchesTable(double amount, bool isReturning, double expected)
    {
        decimal input = (decimal)amount;

        decimal actual = EnrollmentDiscount.For(input, isReturning);

        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1000)]
    public void For_NegativeAmount_ThrowsOutOfRange(double amount)
    {
        decimal input = (decimal)amount;

        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => EnrollmentDiscount.For(input, true));

        Assert.Equal("amount", ex.ParamName);
    }

    [Fact]
    public void For_HugeEnrollment_IsCappedAtMax()
    {
        decimal amount = 5000m;

        decimal actual = EnrollmentDiscount.For(amount, true);

        Assert.Equal(200m, actual);
    }
}
