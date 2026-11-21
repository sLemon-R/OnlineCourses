using System.Text;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Reporting;

Console.OutputEncoding = Encoding.UTF8;

TotalRequest request = BuildRequestFromInput();
TotalResult result = EnrollmentTotalCalculator.Calculate(request);
string report = EnrollmentReport.TextOf(request, result);
new EnrollmentReportPrinter(Console.Out).Print(report);

static TotalRequest BuildRequestFromInput() => new()
{
    EnrollmentId = 1,
    Listener = new ListenerInfo { Name = "Іваненко", Email = "i@ex.com", IsReturning = true },
    Lines = new List<CourseItem>
    {
        new() { CourseCode = "CS-101", Seats = 12, UnitPrice = 100m },
        new() { CourseCode = "DB-201", Seats = 1, UnitPrice = 250m },
    },
    CreatedAt = new DateOnly(2026, 3, 10),
    Status = EnrollmentStatus.New,
    DeliveryPrice = 60m,
};
