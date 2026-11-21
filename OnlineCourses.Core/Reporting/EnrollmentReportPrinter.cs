namespace OnlineCourses.Core.Reporting;

/// <summary>Виведення готового звіту в потік. Нічого не рахує.</summary>
public sealed class EnrollmentReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>Створює друкар для заданого потоку виводу.</summary>
    /// <param name="output">Потік, у який пишеться звіт (консоль, файл, StringWriter).</param>
    public EnrollmentReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Пише звіт у потік.</summary>
    /// <param name="report">Текст звіту.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
