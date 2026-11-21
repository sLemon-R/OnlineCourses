using System.Text;
using Microsoft.Extensions.Logging;
using OnlineCourses.App;
using OnlineCourses.App.Logging;
using OnlineCourses.Core;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
Console.WriteLine($"OnlineCourses v{VersionInfo.Current()}");

string dataPath = Path.Combine("data", "groups.json");
using FileLoggerProvider fileLog = new(Path.Combine("logs", "app.log"));
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(fileLog));

SystemClock clock = new();
InMemoryListenerDirectory listeners = new();
InMemoryGroupRepository groups = new();
SeedData.Fill(listeners, groups, clock.Today.AddDays(7));
if (File.Exists(dataPath))
{
    foreach (CourseGroup saved in new GroupJsonSerializer().FromJson(File.ReadAllText(dataPath)))
    {
        groups.Save(saved);
    }
}

SchoolStores stores = new(groups, listeners, new InMemoryEnrollmentRepository());
GroupEnrollmentService service = new(stores, clock, loggerFactory.CreateLogger<GroupEnrollmentService>());
Console.WriteLine("Слухачів: " + listeners.All().Count + ", груп: " + groups.All().Count);
new Menu(service, stores, clock, dataPath).Run();
