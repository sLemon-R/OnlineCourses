using OnlineCourses.Core.Domain;

namespace OnlineCourses.Tests;

public class EnrollmentTests
{
    [Fact]
    public void Total_NoLines_ReturnsZero()
    {
        // Arrange
        Enrollment enrollment = new() { Id = 1, ListenerId = 10 };

        // Act
        decimal total = enrollment.Total();

        // Assert
        Assert.Equal(0m, total);
    }

    [Fact]
    public void Total_OneSeat_ReturnsUnitPrice()
    {
        Enrollment enrollment = new() { Id = 1, ListenerId = 10 };
        enrollment.AddLine(new EnrollmentLine { CourseCode = "CS-101", Seats = 1, UnitPrice = 899.90m });

        Assert.Equal(899.90m, enrollment.Total());
    }

    [Fact]
    public void Total_TwoSeats_ReturnsDoublePrice()
    {
        Enrollment enrollment = new() { Id = 1, ListenerId = 10 };
        enrollment.AddLine(new EnrollmentLine { CourseCode = "CS-101", Seats = 2, UnitPrice = 1499.50m });

        Assert.Equal(2999.00m, enrollment.Total());
    }

    [Fact]
    public void Total_FiveSeats_ReturnsFivePrices()
    {
        Enrollment enrollment = new() { Id = 1, ListenerId = 10 };
        enrollment.AddLine(new EnrollmentLine { CourseCode = "CS-101", Seats = 5, UnitPrice = 100m });

        Assert.Equal(500m, enrollment.Total());
    }

    [Fact]
    public void Total_TwoLines_ReturnsSumOfLines()
    {
        // Arrange
        Enrollment enrollment = new() { Id = 1, ListenerId = 10 };
        enrollment.AddLine(new EnrollmentLine { CourseCode = "CS-101", Seats = 2, UnitPrice = 1499.50m });
        enrollment.AddLine(new EnrollmentLine { CourseCode = "DB-201", Seats = 1, UnitPrice = 899.90m });

        // Act
        decimal total = enrollment.Total();

        // Assert
        Assert.Equal(3898.90m, total);
    }
}
