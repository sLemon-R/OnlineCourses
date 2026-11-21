using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Services;

namespace OnlineCourses.Tests;

public class DomainInvariantTests
{
    [Fact]
    public void EnrollmentLine_ZeroSeats_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EnrollmentLine("CS-101", 0, 100m));
    }

    [Fact]
    public void ChangeStatus_CompletedToPaid_ThrowsInvalidOperation()
    {
        Enrollment enrollment = new(1, 10, new DateOnly(2026, 11, 21));
        enrollment.AddLine(new EnrollmentLine("CS-101", 1, 100m));
        enrollment.ChangeStatus(EnrollmentStatus.Paid);
        enrollment.ChangeStatus(EnrollmentStatus.InProgress);
        enrollment.ChangeStatus(EnrollmentStatus.Completed);

        Assert.Throws<InvalidOperationException>(
            () => enrollment.ChangeStatus(EnrollmentStatus.Paid));
    }

    [Fact]
    public void TotalOf_DiscountPolicy10Percent_ReturnsDiscountedSum()
    {
        EnrollmentService service = new(
            new InMemoryEnrollmentRepository(), new DiscountPricingPolicy(0.10m));
        Enrollment enrollment = new(1, 10, new DateOnly(2026, 11, 21));
        enrollment.AddLine(new EnrollmentLine("CS-101", 2, 1000m));
        enrollment.AddLine(new EnrollmentLine("DB-201", 1, 500m));

        decimal total = service.TotalOf(enrollment);

        Assert.Equal(2250m, total);
    }
}
