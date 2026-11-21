using Microsoft.Extensions.Logging;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Pricing;

namespace OnlineCourses.Core.Services;

/// <summary>Сценарії зарахування: UC-01 зарахувати, UC-02 відрахувати чи перевести, UC-03 оплатити.</summary>
public sealed partial class GroupEnrollmentService
{
    private const int SeatsPerGroup = 1;

    private readonly SchoolStores _stores;
    private readonly IClock _clock;
    private readonly ILogger<GroupEnrollmentService> _log;

    /// <summary>Створює сервіс. Усі залежності приходять ззовні.</summary>
    /// <param name="stores">Сховища груп, слухачів і заявок.</param>
    /// <param name="clock">Джерело сьогоднішньої дати.</param>
    /// <param name="log">Журнал сценаріїв.</param>
    /// <exception cref="ArgumentNullException">Залежність не задано.</exception>
    public GroupEnrollmentService(SchoolStores stores, IClock clock, ILogger<GroupEnrollmentService> log)
    {
        ArgumentNullException.ThrowIfNull(stores);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(log);
        _stores = stores;
        _clock = clock;
        _log = log;
    }

    /// <summary>UC-01: зараховує слухача до однієї чи кількох груп і створює заявку.</summary>
    /// <param name="listenerId">Ідентифікатор слухача з довідника.</param>
    /// <param name="groupCodes">Коди груп без повторів; хоча б один.</param>
    /// <returns>Номер створеної заявки в стані New.</returns>
    /// <exception cref="DomainRuleException">
    /// Слухача чи групи немає, група стартувала, бракує передумов або місць.
    /// </exception>
    public int Enroll(int listenerId, IReadOnlyList<string> groupCodes)
    {
        ArgumentNullException.ThrowIfNull(groupCodes);
        LogScenarioStarted("UC-01", listenerId);
        Listener listener = FindListener(listenerId);
        List<CourseGroup> groups = ResolveGroups(groupCodes);
        foreach (CourseGroup group in groups)
        {
            EnsureCanJoin(listener, group);
        }

        foreach (CourseGroup group in groups)
        {
            group.Add(listener.Id);
            _stores.Groups.Save(group);
        }

        Enrollment enrollment = CreateEnrollment(listener.Id, groups);
        LogScenarioFinished("UC-01", enrollment.Id);
        return enrollment.Id;
    }

    /// <summary>Рахує суму заявки зі знижкою за пакет курсів.</summary>
    /// <param name="enrollmentId">Номер заявки.</param>
    /// <returns>Сума до сплати, грн.</returns>
    /// <exception cref="DomainRuleException">Заявки чи слухача немає.</exception>
    public decimal TotalOf(int enrollmentId)
    {
        Enrollment enrollment = FindEnrollment(enrollmentId);
        Listener listener = FindListener(enrollment.ListenerId);
        decimal subtotal = enrollment.Lines.Sum(line => line.Amount);
        decimal discount = PackageDiscount.DiscountFor(subtotal, enrollment.Lines.Count, listener.IsReturning);
        return subtotal - discount;
    }

    private void EnsureCanJoin(Listener listener, CourseGroup group)
    {
        if (group.StartDate < _clock.Today)
        {
            LogRejected("UC-01", "group.started");
            throw new DomainRuleException("group.started", "Група " + group.Code + " уже почала навчання");
        }

        string? missing = group.Course.Prerequisites.FirstOrDefault(code => !listener.CompletedCourses.Contains(code));
        if (missing is not null)
        {
            LogRejected("UC-01", "course.prerequisite");
            throw new DomainRuleException("course.prerequisite", "Спершу потрібно пройти курс " + missing);
        }

        if (group.FreeSeats == 0 || group.Contains(listener.Id))
        {
            LogRejected("UC-01", "group.full");
            throw new DomainRuleException("group.full", "У групі " + group.Code + " немає місця для слухача");
        }
    }

    private List<CourseGroup> ResolveGroups(IReadOnlyList<string> groupCodes)
    {
        if (groupCodes.Count == 0)
        {
            throw new DomainRuleException("enrollment.empty", "Потрібно вибрати хоча б одну групу");
        }

        return groupCodes.Distinct(StringComparer.OrdinalIgnoreCase).Select(FindGroup).ToList();
    }

    private Enrollment CreateEnrollment(int listenerId, List<CourseGroup> groups)
    {
        int id = _stores.Enrollments.GetAll().Count + 1;
        Enrollment enrollment = new(id, listenerId, _clock.Today);
        foreach (CourseGroup group in groups)
        {
            enrollment.AddLine(new EnrollmentLine(group.Course.Code, SeatsPerGroup, group.Course.Price));
        }

        _stores.Enrollments.Add(enrollment);
        return enrollment;
    }

    private Listener FindListener(int listenerId) =>
        _stores.Listeners.Find(listenerId)
            ?? throw new DomainRuleException("listener.exists", "Слухача не знайдено");

    private CourseGroup FindGroup(string groupCode) =>
        _stores.Groups.Find(groupCode)
            ?? throw new DomainRuleException("group.exists", "Групу " + groupCode + " не знайдено");

    private Enrollment FindEnrollment(int enrollmentId) =>
        _stores.Enrollments.GetById(enrollmentId)
            ?? throw new DomainRuleException("enrollment.exists", "Заявку не знайдено");

    [LoggerMessage(EventId = 101, Level = LogLevel.Information, Message = "{Scenario}: початок, об'єкт {Id}")]
    private partial void LogScenarioStarted(string scenario, int id);

    [LoggerMessage(EventId = 102, Level = LogLevel.Information, Message = "{Scenario}: завершено, об'єкт {Id}")]
    private partial void LogScenarioFinished(string scenario, int id);

    [LoggerMessage(EventId = 103, Level = LogLevel.Warning, Message = "{Scenario}: відхилено за правилом {Rule}")]
    private partial void LogRejected(string scenario, string rule);
}
