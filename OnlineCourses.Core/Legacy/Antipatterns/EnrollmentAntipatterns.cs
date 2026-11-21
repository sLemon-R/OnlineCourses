#nullable enable
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Legacy.Antipatterns;

/// <summary>Фрагменти з методички ЛР5 з чотирма антипатернами обробки винятків.</summary>
public class EnrollmentAntipatterns
{
    private readonly IEnrollmentRepository _repo;

    /// <summary>Створює клас.</summary>
    public EnrollmentAntipatterns(IEnrollmentRepository repo) => _repo = repo;

    /// <summary>АНТИПАТЕРН 1: порожній catch, дефект зникає.</summary>
    public Enrollment? Load(int enrollmentId)
    {
        try
        {
            return _repo.GetById(enrollmentId);
        }
        catch (Exception)
        {
        }
        return null;
    }

    /// <summary>АНТИПАТЕРНИ 2 і 3: широке перехоплення, throw ex.</summary>
    public void Cancel(int enrollmentId, string reason)
    {
        try
        {
            var enrollment = _repo.GetById(enrollmentId);
            enrollment!.ChangeStatus(EnrollmentStatus.Cancelled);
            _repo.Add(enrollment);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
            throw ex;
        }
    }

    /// <summary>АНТИПАТЕРН 4: try/catch замість звичайної умови.</summary>
    public int CountValidSeats(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            try
            {
                var q = int.Parse(row);
                if (q > 0) valid++;
            }
            catch (FormatException)
            {
                // не число — пропускаємо
            }
        }
        return valid;
    }
}
