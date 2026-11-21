using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

namespace OnlineCourses.Tests;

public class CourseGroupTests
{
    [Fact]
    public void Add_LastFreeSeat_FillsGroup()
    {
        CourseGroup group = TestData.Group("CS-101-A", capacity: 2);
        group.Add(1);

        group.Add(2);

        Assert.Equal(0, group.FreeSeats);
        Assert.Equal(100m, group.FillPercent);
    }

    [Fact]
    public void Add_FullGroup_ThrowsGroupFull()
    {
        CourseGroup group = TestData.Group("CS-101-A", capacity: 1);
        group.Add(1);

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => group.Add(2));

        Assert.Equal("group.full", ex.Rule);
    }

    [Fact]
    public void Add_SameListenerTwice_ThrowsDuplicate()
    {
        CourseGroup group = TestData.Group("CS-101-A", capacity: 5);
        group.Add(1);

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => group.Add(1));

        Assert.Equal("group.duplicate", ex.Rule);
    }

    [Fact]
    public void Remove_UnknownListener_ThrowsMember()
    {
        CourseGroup group = TestData.Group("CS-101-A", capacity: 5);

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => group.Remove(9));

        Assert.Equal("group.member", ex.Rule);
    }

    [Fact]
    public void FillPercent_OneOfThree_RoundsToTenths()
    {
        CourseGroup group = TestData.Group("CS-101-A", capacity: 3);

        group.Add(1);

        Assert.Equal(33.3m, group.FillPercent);
    }

    [Fact]
    public void Constructor_ZeroCapacity_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TestData.Group("X", capacity: 0));
    }
}
