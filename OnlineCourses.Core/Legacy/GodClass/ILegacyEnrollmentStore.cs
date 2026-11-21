namespace OnlineCourses.Core.Legacy.GodClass;

/// <summary>Широкий інтерфейс сховища з методички ЛР4.</summary>
public interface ILegacyEnrollmentStore
{
    /// <summary>Додає заявку.</summary>
    void Add(LegacyEnrollment enrollment);

    /// <summary>Шукає заявку.</summary>
    LegacyEnrollment GetById(int id);

    /// <summary>Зберігає у файл.</summary>
    void SaveToFile(LegacyEnrollment enrollment, string path);

    /// <summary>Надсилає лист.</summary>
    void SendEmail(string to, string body);

    /// <summary>Експортує CSV.</summary>
    string ExportCsv();

    /// <summary>Робить резервну копію.</summary>
    void Backup(string folder);

    /// <summary>Друкує рахунок.</summary>
    void PrintInvoice(LegacyEnrollment enrollment);
}
