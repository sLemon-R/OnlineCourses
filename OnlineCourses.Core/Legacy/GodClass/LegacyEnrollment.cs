namespace OnlineCourses.Core.Legacy.GodClass;

/// <summary>Рядок успадкованої заявки.</summary>
public class LegacyLine
{
    /// <summary>Код курсу.</summary>
    public string CourseCode { get; set; } = "";

    /// <summary>Місць.</summary>
    public int Seats { get; set; }

    /// <summary>Ціна місця.</summary>
    public decimal UnitPrice { get; set; }
}

/// <summary>Успадкована заявка.</summary>
public class LegacyEnrollment
{
    /// <summary>Номер.</summary>
    public int Id { get; set; }

    /// <summary>Стан числом.</summary>
    public int Status { get; set; }

    /// <summary>Сума.</summary>
    public decimal Total { get; set; }

    /// <summary>Пошта.</summary>
    public string Email { get; set; } = "";

    /// <summary>Рядки.</summary>
    public List<LegacyLine> Lines { get; set; } = new();

    /// <summary>Додає рядок.</summary>
    public virtual void AddLine(LegacyLine line) => Lines.Add(line);
}

/// <summary>Архівна заявка, успадкована від звичайної.</summary>
public class ArchivedEnrollment : LegacyEnrollment
{
    /// <summary>Створює архівну заявку.</summary>
    public ArchivedEnrollment(int id) => Id = id;

    /// <summary>Архів змінювати не можна.</summary>
    public override void AddLine(LegacyLine line)
    {
        throw new NotSupportedException("Архів змінювати не можна");
    }
}
