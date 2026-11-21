namespace OnlineCourses.Core.Documents;

/// <summary>Позиція заявки: курс, кількість місць і ціна одного місця.</summary>
public sealed class CourseItem
{
    /// <summary>Отримує або задає код курсу.</summary>
    public string CourseCode { get; set; } = string.Empty;

    /// <summary>Отримує або задає кількість місць.</summary>
    public int Seats { get; set; }

    /// <summary>Отримує або задає ціну одного місця, грн.</summary>
    public decimal UnitPrice { get; set; }
}
