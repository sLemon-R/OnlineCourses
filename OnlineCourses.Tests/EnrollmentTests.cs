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

    [Theory]
    [InlineData(1, 899.90, 899.90)]
    [InlineData(2, 1499.50, 2999.00)]
    [InlineData(5, 100, 500)]
    public void Total_SingleLine_ReturnsSeatsTimesPrice(int seats, decimal unitPrice, decimal expected)
    {
        // Arrange
        Enrollment enrollment = new() { Id = 1, ListenerId = 10 };
        enrollment.AddLine(new EnrollmentLine { CourseCode = "CS-101", Seats = seats, UnitPrice = unitPrice });

        // Act
        decimal total = enrollment.Total();

        // Assert
        Assert.Equal(expected, total);
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
