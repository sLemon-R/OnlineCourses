using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Tests;

public class EnrollmentTotalCalculatorTests
{
    [Fact]
    public void Calculate_RegularListener_Returns1292()
    {
        TotalRequest request = BuildSampleRequest();

        TotalResult result = EnrollmentTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(EnrollmentStatus.Paid, result.Status);
    }

    [Fact]
    public void Calculate_SmallEnrollment_AddsDelivery()
    {
        TotalRequest request = new()
        {
            EnrollmentId = 2,
            Listener = new ListenerInfo { Name = "Петренко", Email = "p@ex.com", IsReturning = false },
            Lines = new List<CourseItem> { new() { CourseCode = "CS-101", Seats = 2, UnitPrice = 200m } },
            CreatedAt = new DateOnly(2026, 3, 10),
            CouponCode = "SALE10",
            Status = EnrollmentStatus.Paid,
            DeliveryPrice = 60m,
        };

        TotalResult result = EnrollmentTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(EnrollmentStatus.InProgress, result.Status);
    }

    [Fact]
    public void Calculate_NoLines_ThrowsArgument()
    {
        TotalRequest request = BuildSampleRequest() with { Lines = new List<CourseItem>() };

        Assert.Throws<ArgumentException>(() => EnrollmentTotalCalculator.Calculate(request));
    }

    internal static TotalRequest BuildSampleRequest() => new()
    {
        EnrollmentId = 1,
        Listener = new ListenerInfo { Name = "Іваненко", Email = "i@ex.com", IsReturning = true },
        Lines = new List<CourseItem>
        {
            new() { CourseCode = "CS-101", Seats = 12, UnitPrice = 100m },
            new() { CourseCode = "DB-201", Seats = 1, UnitPrice = 250m },
        },
        CreatedAt = new DateOnly(2026, 3, 10),
        CouponCode = null,
        Status = EnrollmentStatus.New,
        DeliveryPrice = 60m,
    };
}
