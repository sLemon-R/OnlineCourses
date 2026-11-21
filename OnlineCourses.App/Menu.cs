using System.Globalization;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Reporting;
using OnlineCourses.Core.Services;

namespace OnlineCourses.App;

/// <summary>Текстове меню: читає ввід, викликає сценарії й друкує результат.</summary>
internal sealed class Menu
{
    private readonly GroupEnrollmentService _service;
    private readonly SchoolStores _stores;
    private readonly IClock _clock;
    private readonly string _dataPath;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    /// <summary>Створює меню.</summary>
    /// <param name="service">Сервіс сценаріїв зарахування.</param>
    /// <param name="stores">Сховища для пошуку, звіту й збереження.</param>
    /// <param name="clock">Джерело сьогоднішньої дати для звіту.</param>
    /// <param name="dataPath">Шлях до файлу груп JSON.</param>
    public Menu(GroupEnrollmentService service, SchoolStores stores, IClock clock, string dataPath)
    {
        _service = service;
        _stores = stores;
        _clock = clock;
        _dataPath = dataPath;
        _input = Console.In;
        _output = Console.Out;
    }

    /// <summary>Показує меню, доки користувач не вибере 0 або ввід не закінчиться.</summary>
    public void Run()
    {
        while (true)
        {
            _output.WriteLine();
            _output.WriteLine("1 Зарахувати  2 Відрахувати  3 Перевести  4 Оплатити  5 Пошук  6 Звіт  7 Зберегти  0 Вихід");
            string? choice = Ask("Пункт");
            if (choice is null or "0")
            {
                return;
            }

            try
            {
                Dispatch(choice);
            }
            catch (DomainRuleException ex)
            {
                _output.WriteLine("Відмова [" + ex.Rule + "]: " + ex.Message);
            }
        }
    }

    private static string Num(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static string Money(decimal value) => value.ToString("F2", CultureInfo.InvariantCulture);

    private void Dispatch(string choice)
    {
        Action? action = choice switch
        {
            "1" => Enroll,
            "2" => Expel,
            "3" => Transfer,
            "4" => Pay,
            "5" => Search,
            "6" => Report,
            "7" => Save,
            _ => null,
        };
        if (action is null)
        {
            _output.WriteLine("Невідомий пункт меню");
            return;
        }

        action();
    }

    private void Enroll()
    {
        int listenerId = AskInt("Слухач");
        string[] codes = (Ask("Групи через кому") ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        int id = _service.Enroll(listenerId, codes);
        _output.WriteLine("Заявка № " + Num(id) + ", до сплати " + Money(_service.TotalOf(id)) + " грн");
    }

    private void Expel()
    {
        int listenerId = AskInt("Слухач");
        _service.Expel(listenerId, Ask("Група") ?? string.Empty);
        _output.WriteLine("Відраховано");
    }

    private void Report()
    {
        IReadOnlyList<GroupFillRow> rows = GroupFillReport.Rows(_stores.Groups.All(), _clock.Today);
        _output.Write(GroupFillReport.Text(rows));
    }

    private void Transfer()
    {
        int listenerId = AskInt("Слухач");
        string source = Ask("З групи") ?? string.Empty;
        string target = Ask("До групи") ?? string.Empty;
        _service.Transfer(listenerId, source, target);
        _output.WriteLine("Переведено");
    }

    private void Pay()
    {
        int id = AskInt("Заявка");
        decimal amount = AskMoney("Сума");
        _service.Pay(id, amount);
        _output.WriteLine("Оплачено");
    }

    private void Search()
    {
        string? listener = Ask("Слухач (порожньо — будь-який)");
        string? status = Ask("Стан New/Paid/Cancelled (порожньо — будь-який)");
        int? listenerId = int.TryParse(listener, NumberStyles.None, CultureInfo.InvariantCulture, out int l) ? l : null;
        EnrollmentStatus? state = Enum.TryParse(status, true, out EnrollmentStatus s) ? s : null;
        IReadOnlyList<Enrollment> found = EnrollmentSearch.Find(_stores.Enrollments.GetAll(), listenerId, state);
        _output.WriteLine("Знайдено заявок: " + Num(found.Count));
        foreach (Enrollment e in found)
        {
            _output.WriteLine("№ " + Num(e.Id) + "  слухач " + Num(e.ListenerId) + "  " + e.Status + "  курсів " + Num(e.Lines.Count));
        }
    }

    private void Save()
    {
        string? folder = Path.GetDirectoryName(Path.GetFullPath(_dataPath));
        if (folder is not null)
        {
            Directory.CreateDirectory(folder);
        }

        GroupJsonSerializer serializer = new();
        File.WriteAllText(_dataPath, serializer.ToJson(_stores.Groups.All()));
        _output.WriteLine("Групи збережено у " + _dataPath);
    }

    private string? Ask(string prompt)
    {
        _output.Write(prompt + ": ");
        string? answer = _input.ReadLine();
        _output.WriteLine(answer);
        return answer?.Trim();
    }

    private int AskInt(string prompt)
    {
        while (true)
        {
            string? raw = Ask(prompt);
            if (raw is null)
            {
                throw new DomainRuleException("input.closed", "Ввід завершено");
            }

            if (int.TryParse(raw, NumberStyles.None, CultureInfo.InvariantCulture, out int value) && value > 0)
            {
                return value;
            }

            _output.WriteLine("Помилка вводу: потрібне ціле число більше 0");
        }
    }

    private decimal AskMoney(string prompt)
    {
        while (true)
        {
            string? raw = Ask(prompt);
            if (raw is null)
            {
                throw new DomainRuleException("input.closed", "Ввід завершено");
            }

            if (decimal.TryParse(raw, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal value))
            {
                return value;
            }

            _output.WriteLine("Помилка вводу: сума — число з крапкою, наприклад 4704.00");
        }
    }
}
