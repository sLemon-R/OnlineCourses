namespace OnlineCourses.Core.Legacy.GodClass;

/// <summary>Файлове сховище, яке реалізує весь широкий інтерфейс.</summary>
public class FileEnrollmentStore : ILegacyEnrollmentStore
{
    /// <summary>Не потрібен менеджеру.</summary>
    public void Add(LegacyEnrollment enrollment) => throw new NotImplementedException();

    /// <summary>Не потрібен менеджеру.</summary>
    public LegacyEnrollment GetById(int id) => throw new NotImplementedException();

    /// <summary>Пише заявку у тимчасовий файл.</summary>
    public void SaveToFile(LegacyEnrollment enrollment, string path) =>
        File.WriteAllText(Path.Combine(Path.GetTempPath(), path), enrollment.Id + ";" + enrollment.Total);

    /// <summary>Не потрібен менеджеру.</summary>
    public void SendEmail(string to, string body) => throw new NotImplementedException();

    /// <summary>Не потрібен менеджеру.</summary>
    public string ExportCsv() => throw new NotImplementedException();

    /// <summary>Не потрібен менеджеру.</summary>
    public void Backup(string folder) => throw new NotImplementedException();

    /// <summary>Не потрібен менеджеру.</summary>
    public void PrintInvoice(LegacyEnrollment enrollment) => throw new NotImplementedException();
}

/// <summary>Заглушка поштового клієнта.</summary>
public class SmtpMailer
{
    /// <summary>Імітує надсилання листа.</summary>
    public void Send(string to, string subject, string body)
    {
    }
}
