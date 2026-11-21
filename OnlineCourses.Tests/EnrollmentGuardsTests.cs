using OnlineCourses.Core.Documents;

namespace OnlineCourses.Tests;

public class EnrollmentGuardsTests
{
    [Fact]
    public void EnsureLinesValid_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
            EnrollmentGuards.EnsureLinesValid(new List<CourseItem>()));
    }

    [Fact]
    public void EnsureLineValid_ZeroSeats_ThrowsRange()
    {
        CourseItem line = new() { CourseCode = "CS-101", Seats = 0, UnitPrice = 10m };

        Assert.Throws<ArgumentOutOfRangeException>(() => EnrollmentGuards.EnsureLineValid(line));
    }
}
