using OnlineCourses.Core.Domain;

namespace OnlineCourses.Tests;

public class StatusTransitionTests
{
    [Theory]
    [InlineData(EnrollmentStatus.New, EnrollmentStatus.Paid, true)]
    [InlineData(EnrollmentStatus.New, EnrollmentStatus.Cancelled, true)]
    [InlineData(EnrollmentStatus.New, EnrollmentStatus.Completed, false)]
    [InlineData(EnrollmentStatus.Paid, EnrollmentStatus.InProgress, true)]
    [InlineData(EnrollmentStatus.Paid, EnrollmentStatus.New, false)]
    [InlineData(EnrollmentStatus.Cancelled, EnrollmentStatus.Paid, false)]
    public void CanChangeTo_FromState_MatchesTransitionTable(EnrollmentStatus initial, EnrollmentStatus target, bool expected)
    {
        Enrollment enrollment = TestData.EnrollmentIn(initial);

        bool actual = enrollment.CanChangeTo(target);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ChangeStatus_NewWithoutLinesToPaid_ThrowsInvalidOperation()
    {
        Enrollment enrollment = new(1, 1, new DateOnly(2026, 11, 21));

        Assert.Throws<InvalidOperationException>(() => enrollment.ChangeStatus(EnrollmentStatus.Paid));
    }
}
