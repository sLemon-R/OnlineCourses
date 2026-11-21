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
