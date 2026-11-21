using System.Globalization;
using System.Text;
using OnlineCourses.Core.Domain;

Console.OutputEncoding = Encoding.UTF8;

Enrollment enrollment = new()
{
    Id = 1,
    ListenerId = 10,
    CreatedAt = DateTimeOffset.Now,
};

enrollment.AddLine(new EnrollmentLine
{
    CourseCode = "CS-101",
    Seats = 2,
    UnitPrice = 1499.50m,
});
enrollment.AddLine(new EnrollmentLine
{
    CourseCode = "DB-201",
    Seats = 1,
    UnitPrice = 899.90m,
});

string total = enrollment.Total()
    .ToString("F2", CultureInfo.InvariantCulture);

Console.WriteLine($"Заявка #{enrollment.Id}");
Console.WriteLine($"Стан: {enrollment.Status}");
Console.WriteLine($"Позицій: {enrollment.Lines.Count}");
Console.WriteLine($"Сума: {total}");
