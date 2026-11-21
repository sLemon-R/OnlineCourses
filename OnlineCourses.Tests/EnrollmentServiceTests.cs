using Microsoft.Extensions.Logging.Abstractions;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Services;

namespace OnlineCourses.Tests;

public class EnrollmentServiceTests
{
    [Fact]
    public void TotalOf_TwoLines_SumsPricesFromStub()
    {
        EnrollmentService service = ServiceWith(new FakeEnrollmentRepository(), 25m);
        Enrollment enrollment = TwoLineEnrollment(7);

        decimal actual = service.TotalOf(enrollment);

        Assert.Equal(50m, actual);
    }

    [Fact]
    public void Pay_EnoughAmount_StoresPaidEnrollmentOnce()
    {
        FakeEnrollmentRepository repository = new();
        repository.Seed(TwoLineEnrollment(7));
        EnrollmentService service = ServiceWith(repository, 25m);

        service.Pay(7, 50m);

        Assert.Equal(EnrollmentStatus.Paid, repository.GetById(7)!.Status);
        Assert.Equal(1, repository.AddCalls);
    }

    [Fact]
    public void Pay_UnknownId_ThrowsDomainRule()
    {
        EnrollmentService service = ServiceWith(new FakeEnrollmentRepository(), 25m);

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => service.Pay(99, 10m));

        Assert.Equal("enrollment.exists", ex.Rule);
    }

    [Fact]
    public void Pay_AmountBelowTotal_ThrowsAndKeepsNew()
    {
        FakeEnrollmentRepository repository = new();
        repository.Seed(TwoLineEnrollment(7));
        EnrollmentService service = ServiceWith(repository, 25m);

        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => service.Pay(7, 49.99m));

        Assert.Equal("enrollment.amount", ex.Rule);
        Assert.Equal(EnrollmentStatus.New, repository.GetById(7)!.Status);
        Assert.Equal(0, repository.AddCalls);
    }

    [Fact]
    public void Pay_Overpayment_StillMarksPaid()
    {
        FakeEnrollmentRepository repository = new();
        repository.Seed(TwoLineEnrollment(7));
        EnrollmentService service = ServiceWith(repository, 25m);

        service.Pay(7, 80m);

        Assert.Equal(EnrollmentStatus.Paid, repository.GetById(7)!.Status);
    }

    [Fact]
    public void Cancel_NewEnrollment_SetsCancelled()
    {
        FakeEnrollmentRepository repository = new();
        repository.Seed(TwoLineEnrollment(7));
        EnrollmentService service = ServiceWith(repository, 25m);

        service.Cancel(7, "передумав");

        Assert.Equal(EnrollmentStatus.Cancelled, repository.GetById(7)!.Status);
    }

    [Fact]
    public void Cancel_BlankReason_ThrowsArgument()
    {
        EnrollmentService service = ServiceWith(new FakeEnrollmentRepository(), 25m);

        Assert.Throws<ArgumentException>(() => service.Cancel(7, " "));
    }

    [Fact]
    public void AddLine_ExistingEnrollment_AddsAndStores()
    {
        FakeEnrollmentRepository repository = new();
        repository.Seed(TwoLineEnrollment(7));
        EnrollmentService service = ServiceWith(repository, 25m);

        service.AddLine(7, new EnrollmentLine("AI-301", 1, 900m));

        Assert.Equal(3, repository.GetById(7)!.Lines.Count);
        Assert.Equal(1, repository.AddCalls);
    }

    [Fact]
    public void FindOrDefault_UnknownId_ReturnsNull()
    {
        EnrollmentService service = ServiceWith(new FakeEnrollmentRepository(), 25m);

        Enrollment? actual = service.FindOrDefault(99);

        Assert.Null(actual);
    }

    private static Enrollment TwoLineEnrollment(int id)
    {
        Enrollment enrollment = new(id, 10, new DateOnly(2026, 11, 21));
        enrollment.AddLine(new EnrollmentLine("CS-101", 2, 10m));
        enrollment.AddLine(new EnrollmentLine("DB-201", 1, 30m));
        return enrollment;
    }

    private static EnrollmentService ServiceWith(IEnrollmentRepository repository, decimal price) =>
        new(repository, new StubPricingPolicy(price), NullLogger<EnrollmentService>.Instance);
}
