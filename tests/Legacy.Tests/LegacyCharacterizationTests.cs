using System.Globalization;
using OnlineCourses.Legacy;

namespace OnlineCourses.Legacy.Tests;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    private static List<CourseLine> TwoLines() => new()
    {
        new CourseLine { Code = "A-1", Seats = 2, Price = 150m },
        new CourseLine { Code = "B-2", Seats = 1, Price = 400m }
    };

    [Theory]
    [InlineData(null, DocState.New, "a@b.c", "ERR: null")]
    [InlineData("empty", DocState.New, "a@b.c", "ERR: empty")]
    [InlineData("two", DocState.Draft, "a@b.c", "ERR: state")]
    [InlineData("two", DocState.New, "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfItems, DocState state, string mail, string expected)
    {
        var items = kindOfItems switch
        {
            "two" => TwoLines(),
            "empty" => new List<CourseLine>(),
            _ => null
        };
        var sut = new LegacyEnrollmentProcessor();

        var actual = sut.Handle(1001, 7, "Іван", mail,
            ClientKind.Regular, 0, items, state, "UAH",
            new DateTime(2026, 3, 1), false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipEnrollment_ReturnsExactReport()
    {
        var sut = new LegacyEnrollmentProcessor();

        var report = sut.Handle(1001, 7, "Іван", "a@b.c",
            ClientKind.Vip, 3, TwoLines(), DocState.New, "UAH",
            new DateTime(2026, 3, 1), false);

        var expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "A-1 x2 = 300.00 UAH\n" +
            "B-2 x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";
        Assert.Equal(expected, report);
    }

    [Theory]
    [InlineData(ClientKind.Regular, 0, 760)]
    [InlineData(ClientKind.Vip, 0, 655)]
    [InlineData(ClientKind.Staff, 0, 550)]
    [InlineData(ClientKind.Regular, 11, 725)]
    public void Preview_ByKind_ReturnsTotal(ClientKind kind, int done, decimal expected)
    {
        var sut = new LegacyEnrollmentProcessor();

        var total = sut.Preview(kind, done, TwoLines());

        Assert.Equal(expected, total);
    }

    [Fact]
    public void Preview_PayableExactlyAtFreeShippingBorder_NoShipping()
    {
        var sut = new LegacyEnrollmentProcessor();
        var items = new List<CourseLine>
        {
            new CourseLine { Code = "C-3", Seats = 1, Price = 1000m }
        };

        var total = sut.Preview(ClientKind.Regular, 0, items);

        Assert.Equal(1000m, total);
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var c = new Learner
        {
            Name = " іван ",
            Email = "IVAN@MAIL.COM",
            Kind = ClientKind.Vip,
            DoneCount = 12,
            SinceUtc = new DateTime(2020, 1, 1)
        };
        var years = DateTime.Now.Year - 2020;
        var expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years;

        var actual = new LegacyEnrollmentProcessor().DescribeClient(c);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DumpLog_AfterHandleWithMail_ListsBothRecords()
    {
        var sut = new LegacyEnrollmentProcessor();

        sut.Handle(1001, 7, "Іван", "a@b.c",
            ClientKind.Regular, 0, TwoLines(), DocState.New, "UAH",
            new DateTime(2026, 3, 1), true);

        Assert.Equal("ok 1001\nmail -> a@b.c\n", sut.DumpLog());
    }
}
