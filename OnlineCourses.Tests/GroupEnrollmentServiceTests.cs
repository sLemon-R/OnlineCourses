using Microsoft.Extensions.Logging.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Services;

namespace OnlineCourses.Tests;

public class GroupEnrollmentServiceTests
{
    private readonly InMemoryGroupRepository _groups = new();
    private readonly InMemoryListenerDirectory _listeners = new();
    private readonly FakeEnrollmentRepository _enrollments = new();
    private readonly GroupEnrollmentService _service;

    public GroupEnrollmentServiceTests()
    {
        Course basics = TestData.Course("CS-101", 2400m);
        Course oop = TestData.Course("CS-201", 3200m, "CS-101");
        Course sql = TestData.Course("DB-101", 2800m);
        _groups.Save(new CourseGroup("CS-101-A", basics, 2, TestData.Today.AddDays(7)));
        _groups.Save(new CourseGroup("CS-101-B", basics, 1, TestData.Today.AddDays(21)));
        _groups.Save(new CourseGroup("CS-201-A", oop, 10, TestData.Today.AddDays(7)));
        _groups.Save(new CourseGroup("DB-101-A", sql, 10, TestData.Today.AddDays(7)));
        _groups.Save(new CourseGroup("DB-101-OLD", sql, 10, TestData.Today.AddDays(-1)));
        _listeners.Add(new Listener(1, "Новак Іра", "ira@example.com", false));
        _listeners.Add(new Listener(2, "Коваль Олег", "oleg@example.com", true)
        {
            CompletedCourses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CS-101" },
        });
        SchoolStores stores = new(_groups, _listeners, _enrollments);
        _service = new GroupEnrollmentService(stores, new FixedClock(TestData.Today), NullLogger<GroupEnrollmentService>.Instance);
    }

    [Fact]
    public void Enroll_TwoGroups_CreatesEnrollmentWithPackageDiscount()
    {
        int id = _service.Enroll(1, new[] { "CS-101-A", "DB-101-A" });

        Assert.Equal(4940m, _service.TotalOf(id));
        Assert.Equal(1, _groups.Find("CS-101-A")!.Occupied);
        Assert.Equal(EnrollmentStatus.New, _enrollments.GetById(id)!.Status);
    }

    [Fact]
    public void Enroll_ReturningListenerWithPrerequisite_GetsBonus()
    {
        int id = _service.Enroll(2, new[] { "CS-201-A" });

        Assert.Equal(3104m, _service.TotalOf(id));
    }

    [Fact]
    public void Enroll_MissingPrerequisite_ThrowsAndTakesNoSeat()
    {
        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => _service.Enroll(1, new[] { "DB-101-A", "CS-201-A" }));

        Assert.Equal("course.prerequisite", ex.Rule);
        Assert.Equal(0, _groups.Find("DB-101-A")!.Occupied);
        Assert.Empty(_enrollments.GetAll());
    }

    [Fact]
    public void Enroll_StartedGroup_ThrowsStarted()
    {
        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => _service.Enroll(1, new[] { "DB-101-OLD" }));

        Assert.Equal("group.started", ex.Rule);
    }

    [Fact]
    public void Enroll_UnknownListener_ThrowsListenerExists()
    {
        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => _service.Enroll(99, new[] { "CS-101-A" }));

        Assert.Equal("listener.exists", ex.Rule);
    }

    [Fact]
    public void Enroll_NoGroups_ThrowsEmpty()
    {
        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => _service.Enroll(1, Array.Empty<string>()));

        Assert.Equal("enrollment.empty", ex.Rule);
    }

    [Fact]
    public void Enroll_FullGroup_ThrowsFull()
    {
        _service.Enroll(1, new[] { "CS-101-B" });

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => _service.Enroll(2, new[] { "CS-101-B" }));

        Assert.Equal("group.full", ex.Rule);
    }

    [Fact]
    public void Transfer_SameCourse_MovesSeat()
    {
        _service.Enroll(1, new[] { "CS-101-A" });

        _service.Transfer(1, "CS-101-A", "CS-101-B");

        Assert.False(_groups.Find("CS-101-A")!.Contains(1));
        Assert.True(_groups.Find("CS-101-B")!.Contains(1));
    }

    [Fact]
    public void Transfer_ToFullGroup_KeepsOriginalSeat()
    {
        _service.Enroll(2, new[] { "CS-101-B" });
        _service.Enroll(1, new[] { "CS-101-A" });

        Assert.Throws<DomainRuleException>(() => _service.Transfer(1, "CS-101-A", "CS-101-B"));

        Assert.True(_groups.Find("CS-101-A")!.Contains(1));
    }

    [Fact]
    public void Transfer_OtherCourse_ThrowsCourse()
    {
        _service.Enroll(1, new[] { "CS-101-A" });

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => _service.Transfer(1, "CS-101-A", "DB-101-A"));

        Assert.Equal("group.course", ex.Rule);
    }

    [Fact]
    public void Expel_EnrolledListener_FreesSeat()
    {
        _service.Enroll(1, new[] { "CS-101-A" });

        _service.Expel(1, "CS-101-A");

        Assert.Equal(2, _groups.Find("CS-101-A")!.FreeSeats);
    }

    [Fact]
    public void Pay_FullAmount_MarksPaid()
    {
        int id = _service.Enroll(1, new[] { "CS-101-A" });

        _service.Pay(id, 2400m);

        Assert.Equal(EnrollmentStatus.Paid, _enrollments.GetById(id)!.Status);
    }

    [Fact]
    public void Pay_Twice_ThrowsState()
    {
        int id = _service.Enroll(1, new[] { "CS-101-A" });
        _service.Pay(id, 2400m);

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => _service.Pay(id, 2400m));

        Assert.Equal("enrollment.state", ex.Rule);
    }

    [Fact]
    public void Pay_LessThanTotal_ThrowsAmount()
    {
        int id = _service.Enroll(1, new[] { "CS-101-A" });

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => _service.Pay(id, 2399.99m));

        Assert.Equal("enrollment.amount", ex.Rule);
    }
}
