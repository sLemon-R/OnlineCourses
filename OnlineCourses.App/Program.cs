using System.Text;
using OnlineCourses.App;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Reporting;
using OnlineCourses.Core.Services;

Console.OutputEncoding = Encoding.UTF8;

// композиційний корінь: тільки тут створюються конкретні реалізації
Dictionary<string, IPricingPolicy> policies = new()
{
    ["returning"] = new DiscountPricingPolicy(0.07m),
    ["corporate"] = new GroupPricingPolicy(),
};
InMemoryEnrollmentRepository repository = new();

Listener listener = new(10, "Іваненко Олена", "olena@ex.com", isReturning: true);
string listenerType = listener.IsReturning ? "returning" : "standard";
IPricingPolicy policy = policies.TryGetValue(listenerType, out IPricingPolicy? found)
    ? found
    : new StandardPricingPolicy();
EnrollmentService service = new(repository, policy);
EnrollmentConfirmation confirmation = new(new ConsoleNotifier());

Enrollment enrollment = new(1, listener.Id, new DateOnly(2026, 11, 21));
enrollment.AddLine(new EnrollmentLine("CS-101", 2, 1499.50m));
enrollment.AddLine(new EnrollmentLine("DB-201", 1, 899.90m));

decimal total = service.Place(enrollment);
confirmation.Send(listener, enrollment, total);
Console.WriteLine($"Заявка № {enrollment.Id}, слухач {listener.FullName}");
Console.WriteLine($"Політика: {policy.GetType().Name}");
Console.WriteLine($"Вартість: {total:F2}");
Console.WriteLine($"Стан: {enrollment.Status}");
Console.WriteLine($"Заявок у сховищі: {repository.GetAll().Count}");
Console.Write(EnrollmentCsvReport.TextOf(repository.GetAll()));
