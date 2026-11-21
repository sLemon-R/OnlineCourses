using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Input;

namespace OnlineCourses.Tests;

public class EnrollmentLineParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("CS-101;2")]
    [InlineData("CS-101;0;10.50")]
    [InlineData("CS-101;-2;10.50")]
    [InlineData("CS-101;2;-10.50")]
    [InlineData("CS-101;2;1499,50")]
    [InlineData("CS-101;1001;10.50")]
    [InlineData("ДУЖЕ-ДОВГИЙ-КОД-КУРСУ-21;1;10")]
    public void ParseRow_InvalidInput_ReturnsFailure(string raw)
    {
        Result<EnrollmentLine> actual = EnrollmentLineParser.ParseRow(raw);

        Assert.False(actual.IsSuccess);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void ParseRow_ValidInput_ReturnsLine()
    {
        Result<EnrollmentLine> actual = EnrollmentLineParser.ParseRow(" CS-101 ;2;1499.50");

        Assert.True(actual.IsSuccess);
        Assert.Equal(new EnrollmentLine("CS-101", 2, 1499.50m), actual.Value);
    }
}
