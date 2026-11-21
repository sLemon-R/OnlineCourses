using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Reporting;
using OnlineCourses.Core.Services;

namespace OnlineCourses.Tests;

public class DomainCoverageTests
{
    [Theory]
    [InlineData(4, 400)]
    [InlineData(5, 425)]
    public void GroupPolicy_SeatsAroundGroupBorder_AppliesRateFromFive(int seats, double expected)
    {
        GroupPricingPolicy policy = new();

        decimal actual = policy.PriceOf(new EnrollmentLine("CS-101", seats, 100m));

        Assert.Equal((decimal)expected, actual);
    }

    [Fact]
    public void StandardPolicy_Line_ReturnsAmount()
    {
        StandardPricingPolicy policy = new();

        decimal actual = policy.PriceOf(new EnrollmentLine("CS-101", 3, 33.335m));

        Assert.Equal(100.01m, actual);
    }

    [Fact]
    public void ArchivedEnrollment_NewEnrollment_ThrowsInvalidOperation()
    {
        Enrollment enrollment = new(1, 10, new DateOnly(2026, 11, 21));

        Assert.Throws<InvalidOperationException>(
            () => new ArchivedEnrollment(enrollment, new DateOnly(2026, 12, 1)));
    }

    [Fact]
    public void ArchivedEnrollment_Cancelled_KeepsIdAndStatus()
    {
        Enrollment enrollment = new(3, 10, new DateOnly(2026, 11, 21));
        enrollment.ChangeStatus(EnrollmentStatus.Cancelled);

        ArchivedEnrollment archived = new(enrollment, new DateOnly(2026, 12, 1));

        Assert.Equal(3, archived.Id);
        Assert.Equal(EnrollmentStatus.Cancelled, archived.Status);
        Assert.Empty(archived.Lines);
    }

    [Fact]
    public void CsvReport_OneEnrollment_WritesHeaderAndRow()
    {
        Enrollment enrollment = new(5, 10, new DateOnly(2026, 11, 21));
        enrollment.AddLine(new EnrollmentLine("CS-101", 1, 100m));

        string actual = EnrollmentCsvReport.TextOf(new[] { enrollment });

        Assert.Equal("id;status;lines" + Environment.NewLine + "5;New;1" + Environment.NewLine, actual);
    }

    [Fact]
    public void Confirmation_Send_NotifiesListenerOnce()
    {
        SpyNotifier notifier = new();
        EnrollmentConfirmation confirmation = new(notifier);
        Listener listener = new(10, "Іваненко Іван", "ivan@example.com", false);
        Enrollment enrollment = new(5, 10, new DateOnly(2026, 11, 21));

        confirmation.Send(listener, enrollment, 1499.5m);

        Assert.Equal("ivan@example.com|Заявка № 5|До сплати: 1499.50 грн", Assert.Single(notifier.Sent));
    }

    [Fact]
    public void Listener_EmailWithoutAt_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() => new Listener(10, "Іваненко Іван", "ivan.example.com", false));
    }

    [Fact]
    public void Result_OkAndFail_CarryValueOrError()
    {
        Result<int> ok = Result.Ok(5);
        Result<int> fail = Result.Fail<int>("погано");

        Assert.True(ok.IsSuccess);
        Assert.Equal(5, ok.Value);
        Assert.False(fail.IsSuccess);
        Assert.Equal("погано", fail.Error);
    }
}
