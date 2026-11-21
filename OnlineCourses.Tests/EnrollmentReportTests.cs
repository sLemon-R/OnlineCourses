using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Reporting;

namespace OnlineCourses.Tests;

public class EnrollmentReportTests
{
    [Fact]
    public void TextOf_ContainsTotalLine()
    {
        TotalRequest request = EnrollmentTotalCalculatorTests.BuildSampleRequest();
        TotalResult result = EnrollmentTotalCalculator.Calculate(request);

        string text = EnrollmentReport.TextOf(request, result);

        Assert.Contains("Разом: 1292.70", text);
    }

    [Fact]
    public void Print_WritesReportToOutput()
    {
        StringWriter writer = new();
        EnrollmentReportPrinter printer = new(writer);

        printer.Print("Разом: 10.00");

        Assert.Contains("10.00", writer.ToString());
    }

    [Fact]
    public void CouponDiscountOf_Minus200BelowThreshold_ReturnsZero()
    {
        decimal discount = EnrollmentTotalCalculator.CouponDiscountOf(900m, "MINUS200");

        Assert.Equal(0m, discount);
    }
}
