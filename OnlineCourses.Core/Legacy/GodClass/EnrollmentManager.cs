namespace OnlineCourses.Core.Legacy.GodClass;

/// <summary>Клас-Бог з методички ЛР4 для аудиту SOLID.</summary>
public class EnrollmentManager
{
    private readonly FileEnrollmentStore _store = new FileEnrollmentStore();
    private readonly SmtpMailer _mailer = new SmtpMailer();

    /// <summary>Оформлює заявку.</summary>
    public decimal Place(LegacyEnrollment enrollment, string listenerType)
    {
        decimal total = 0m;
        foreach (var line in enrollment.Lines)
            total += line.UnitPrice * line.Seats;
        if (listenerType == "regular")
            total = total * 0.95m;
        else if (listenerType == "vip")
            total = total * 0.90m;
        else if (listenerType == "staff")
            total = total * 0.70m;
        if (total > 1000m)
            total = total - 50m;
        enrollment.Total = total;
        enrollment.Status = 1;
        _store.SaveToFile(enrollment, "enrollments.json");
        _mailer.Send(enrollment.Email, "Enrollment " + enrollment.Id, "" + total);
        return total;
    }

    /// <summary>Формує CSV-звіт.</summary>
    public string BuildCsvReport(List<LegacyEnrollment> enrollments)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var o in enrollments)
            text += o.Id + ";" + o.Total + ";" + o.Status;
        File.WriteAllText("report.csv", text);
        Console.WriteLine("Звіт збережено");
        return text;
    }

    /// <summary>Архівує заявку.</summary>
    public void Archive(LegacyEnrollment enrollment)
    {
        LegacyEnrollment archived = new ArchivedEnrollment(enrollment.Id);
        foreach (var line in enrollment.Lines)
            archived.AddLine(line);
        _store.SaveToFile(archived, "archive.json");
    }
}
