using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Tests;

public class LegacyProcessorTests
{
    [Fact]
    public void Process_RegularListener_Returns1292()
    {
        List<CourseItem> lines = new()
        {
            new CourseItem { CourseCode = "CS-101", Seats = 12, UnitPrice = 100m },
            new CourseItem { CourseCode = "DB-201", Seats = 1, UnitPrice = 250m },
        };

        decimal total = EnrollmentTotalCalculator.Process(
            1, "Іваненко", "i@ex.com", true, lines,
            new DateOnly(2026, 3, 10), "", EnrollmentStatus.New, false,
            60m, out EnrollmentStatus next);

        Assert.Equal(1292.70m, total);
        Assert.Equal(EnrollmentStatus.Paid, next);
    }

    [Fact]
    public void Process_SmallEnrollment_AddsDelivery()
    {
        List<CourseItem> lines = new()
        {
            new CourseItem { CourseCode = "CS-101", Seats = 2, UnitPrice = 200m },
        };

        decimal total = EnrollmentTotalCalculator.Process(
            2, "Петренко", "p@ex.com", false, lines,
            new DateOnly(2026, 3, 10), "SALE10", EnrollmentStatus.Paid,
            false, 60m, out EnrollmentStatus next);

        Assert.Equal(420m, total);
        Assert.Equal(EnrollmentStatus.InProgress, next);
    }

    [Fact]
    public void Process_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() => EnrollmentTotalCalculator.Process(
            3, "Коваль", "k@ex.com", false,
            new List<CourseItem>(),
            new DateOnly(2026, 3, 10), "", EnrollmentStatus.New, false,
            60m, out EnrollmentStatus next));
    }
}
