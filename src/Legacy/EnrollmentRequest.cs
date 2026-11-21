namespace OnlineCourses.Legacy;

public sealed record EnrollmentRequest(
    int DocId,
    Learner Client,
    IReadOnlyList<CourseLine>? Items,
    DocState State,
    string Currency,
    bool SendMail);
