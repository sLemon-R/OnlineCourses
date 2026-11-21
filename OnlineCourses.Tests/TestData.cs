using OnlineCourses.Core.Domain;

namespace OnlineCourses.Tests;

internal static class TestData
{
    public static readonly DateOnly Today = new(2026, 11, 21);

    public static Course Course(string code, decimal price, params string[] prerequisites) => new()
    {
        Code = code,
        Title = "Курс " + code,
        Price = price,
        Prerequisites = prerequisites,
    };

    public static CourseGroup Group(string code, int capacity) =>
        new(code, Course(code[..Math.Min(code.Length, 6)], 1000m), capacity, Today.AddDays(7));

    public static Enrollment EnrollmentIn(EnrollmentStatus status)
    {
        Enrollment enrollment = new(1, 1, Today);
        enrollment.AddLine(new EnrollmentLine("CS-101", 1, 1000m));
        if (status is EnrollmentStatus.Paid or EnrollmentStatus.InProgress)
        {
            enrollment.ChangeStatus(EnrollmentStatus.Paid);
        }

        if (status == EnrollmentStatus.InProgress)
        {
            enrollment.ChangeStatus(EnrollmentStatus.InProgress);
        }

        if (status == EnrollmentStatus.Cancelled)
        {
            enrollment.ChangeStatus(EnrollmentStatus.Cancelled);
        }

        return enrollment;
    }
}
