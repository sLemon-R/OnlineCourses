using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Tests;

internal sealed class FakeEnrollmentRepository : IEnrollmentRepository
{
    private readonly Dictionary<int, Enrollment> _items = new();

    public int AddCalls { get; private set; }

    public void Seed(Enrollment enrollment) => _items[enrollment.Id] = enrollment;

    public void Add(Enrollment enrollment)
    {
        AddCalls++;
        _items[enrollment.Id] = enrollment;
    }

    public Enrollment? GetById(int id) =>
        _items.TryGetValue(id, out Enrollment? found) ? found : null;

    public IReadOnlyList<Enrollment> GetAll() => _items.Values.ToList();
}

internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price) => _price = price;

    public decimal PriceOf(EnrollmentLine line) => _price;
}

internal sealed class SpyNotifier : INotifier
{
    public List<string> Sent { get; } = new();

    public void Notify(string recipient, string subject, string body) =>
        Sent.Add(recipient + "|" + subject + "|" + body);
}
