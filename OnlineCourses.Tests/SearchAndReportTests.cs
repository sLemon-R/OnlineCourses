using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Reporting;
using OnlineCourses.Core.Services;

namespace OnlineCourses.Tests;

public class SearchAndReportTests
{
    [Fact]
    public void Find_NoEnrollments_ReturnsEmpty()
    {
        IReadOnlyList<Enrollment> actual = EnrollmentSearch.Find(Array.Empty<Enrollment>(), 1, null);

        Assert.Empty(actual);
    }

    [Fact]
    public void Find_ByListenerAndStatus_ReturnsMatchingInIdOrder()
    {
        Enrollment third = Make(3, 7);
        Enrollment first = Make(1, 7);
        Enrollment other = Make(2, 8);
        Enrollment cancelled = Make(4, 7);
        cancelled.ChangeStatus(EnrollmentStatus.Cancelled);

        IReadOnlyList<Enrollment> actual = EnrollmentSearch.Find(new[] { third, other, cancelled, first }, 7, EnrollmentStatus.New);

        Assert.Equal(new[] { 1, 3 }, actual.Select(e => e.Id));
    }

    [Fact]
    public void Rows_SeveralGroups_SortedByFillThenCode()
    {
        CourseGroup half = TestData.Group("B-GROUP", capacity: 2);
        half.Add(1);
        CourseGroup full = TestData.Group("C-GROUP", capacity: 1);
        full.Add(1);
        CourseGroup sameHalf = TestData.Group("A-GROUP", capacity: 2);
        sameHalf.Add(2);

        IReadOnlyList<GroupFillRow> rows = GroupFillReport.Rows(new[] { half, full, sameHalf }, TestData.Today);

        Assert.Equal(new[] { "C-GROUP", "A-GROUP", "B-GROUP" }, rows.Select(r => r.GroupCode));
    }

    [Fact]
    public void Rows_GroupStartedBeforeDate_IsExcluded()
    {
        CourseGroup group = TestData.Group("A-GROUP", capacity: 2);

        IReadOnlyList<GroupFillRow> rows = GroupFillReport.Rows(new[] { group }, TestData.Today.AddDays(30));

        Assert.Empty(rows);
    }

    [Fact]
    public void Text_OneRow_ContainsPercent()
    {
        CourseGroup group = TestData.Group("A-GROUP", capacity: 4);
        group.Add(1);

        string text = GroupFillReport.Text(GroupFillReport.Rows(new[] { group }, TestData.Today));

        Assert.Contains("25.0 %", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Text_NoRows_SaysNoGroups()
    {
        string text = GroupFillReport.Text(Array.Empty<GroupFillRow>());

        Assert.StartsWith("Груп для звіту немає", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Json_RoundTrip_KeepsGroupAndListeners()
    {
        CourseGroup group = new("CS-201-A", TestData.Course("CS-201", 3200m, "CS-101"), 10, TestData.Today);
        group.Add(5);
        group.Add(2);
        GroupJsonSerializer serializer = new();

        CourseGroup restored = Assert.Single(serializer.FromJson(serializer.ToJson(new[] { group })));

        Assert.Equal("CS-201-A", restored.Code);
        Assert.Equal(3200m, restored.Course.Price);
        Assert.Equal(new[] { "CS-101" }, restored.Course.Prerequisites);
        Assert.Equal(new[] { 2, 5 }, restored.Listeners.OrderBy(id => id));
    }

    private static Enrollment Make(int id, int listenerId)
    {
        Enrollment enrollment = new(id, listenerId, TestData.Today);
        enrollment.AddLine(new EnrollmentLine("CS-101", 1, 100m));
        return enrollment;
    }
}
