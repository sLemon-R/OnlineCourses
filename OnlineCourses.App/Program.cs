using System.Text;
using Microsoft.Extensions.Logging;
using OnlineCourses.App;
using OnlineCourses.Core;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Input;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Reporting;
using OnlineCourses.Core.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
Console.WriteLine($"OnlineCourses v{VersionInfo.Current()}");

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
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
        options.SingleLine = true;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});
ILogger<EnrollmentService> serviceLogger = loggerFactory.CreateLogger<EnrollmentService>();
EnrollmentService service = new(repository, policy, serviceLogger);
EnrollmentConfirmation confirmation = new(new ConsoleNotifier());

Enrollment enrollment = new(1, listener.Id, new DateOnly(2026, 11, 21));
repository.Add(enrollment);

Console.WriteLine("Позиція: код;місця;ціна (порожній рядок — кінець вводу)");
while (true)
{
    Console.Write("> ");
    string? input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }

    Console.WriteLine(input);
    Result<EnrollmentLine> parsed = EnrollmentLineParser.ParseRow(input);
    if (!parsed.IsSuccess)
    {
        Console.WriteLine("Помилка вводу: " + parsed.Error);
        continue;
    }

    service.AddLine(enrollment.Id, parsed.Value!);
    Console.WriteLine("Додано: " + parsed.Value!.CourseCode);
}

string auditPath = Path.Combine(Path.GetTempPath(), "onlinecourses_audit.csv");
decimal total = service.TotalOf(enrollment);
using (FileAuditLog audit = new(auditPath))
{
    service.Pay(enrollment.Id, total);
    audit.Write("pay", enrollment.Id);
}

// файл закривається навіть після винятку всередині using
try
{
    using FileAuditLog audit = new(auditPath);
    audit.Write("cancel-attempt", 42);
    service.Cancel(42, "тест");
}
catch (DomainRuleException ex)
{
    Console.WriteLine("Відмова [" + ex.Rule + "]: " + ex.Message);
}

using (FileAuditLog reopened = new(auditPath))
{
    reopened.Write("reopen-ok", enrollment.Id);
    Console.WriteLine("Журнал аудиту відкрито повторно: файл не заблоковано");
}

confirmation.Send(listener, enrollment, total);
Console.WriteLine($"Заявка № {enrollment.Id}: {total:F2} грн, стан {enrollment.Status}");

try
{
    service.Pay(enrollment.Id, 1m);
}
catch (DomainRuleException ex)
{
    Console.WriteLine("Відмова [" + ex.Rule + "]: " + ex.Message);
}

try
{
    service.Pay(42, 100m);
}
catch (DomainRuleException ex)
{
    Console.WriteLine("Відмова [" + ex.Rule + "]: " + ex.Message);
}

string[] seatSamples = { "2", "дві", "-1", "5" };
Console.WriteLine("Коректних рядків місць: " + EnrollmentLineParser.CountValidSeats(seatSamples));

try
{
    service.Cancel(enrollment.Id, "передумав");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine("Скасування неможливе: " + ex.Message);
}

Console.Write(EnrollmentCsvReport.TextOf(repository.GetAll()));
