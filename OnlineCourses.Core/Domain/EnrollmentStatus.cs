namespace OnlineCourses.Core.Domain;

/// <summary>Стан заявки на навчання в її життєвому циклі.</summary>
public enum EnrollmentStatus
{
    New = 0,
    Paid = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
}
