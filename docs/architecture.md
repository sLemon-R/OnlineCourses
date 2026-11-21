# Архітектура OnlineCourses

## Шари

| Шар | Де в коді | Що містить | Що йому заборонено |
|---|---|---|---|
| Домен | OnlineCourses.Core/Domain, Errors, Pricing | Course, CourseGroup, Listener, Enrollment, EnrollmentStatus, PackageDiscount, DomainRuleException | знати про файли, консоль, JSON |
| Прикладний | OnlineCourses.Core/Services, Abstractions, Reporting, Input | GroupEnrollmentService, EnrollmentSearch, GroupFillReport, IGroupRepository, IListenerDirectory, IClock | знати формат зберігання й консоль |
| Інфраструктура | OnlineCourses.Core/Infrastructure | InMemoryGroupRepository, InMemoryListenerDirectory, SystemClock, GroupJsonSerializer, FileAuditLog | містити правила домену |
| Застосунок | OnlineCourses.App | Program (композиційний корінь), Menu, SeedData, FileLoggerProvider | містити розрахунки й правила |

Вибрано варіант із трьома проєктами (Core, App, Tests) і шарами-теками в Core.

## Залежності

```
OnlineCourses.App  ->  Services / Abstractions  ->  Domain
                              ^
                              |
                        Infrastructure
```

- App -> прикладний шар: меню викликає сценарії GroupEnrollmentService і не рахує нічого сам.
- Прикладний -> домен: сервіс керує групами й заявками, але правила (місця, переходи, знижка) живуть у доменних класах.
- Інфраструктура -> прикладний: InMemoryGroupRepository і SystemClock реалізують інтерфейси, оголошені в Abstractions.
- Домен не посилається ні на що: у Domain немає using на Infrastructure, System.IO чи System.Text.Json.

## Композиційний корінь (Program.cs)

```csharp
SystemClock clock = new();
InMemoryListenerDirectory listeners = new();
InMemoryGroupRepository groups = new();
SeedData.Fill(listeners, groups, clock.Today.AddDays(7));
SchoolStores stores = new(groups, listeners, new InMemoryEnrollmentRepository());
GroupEnrollmentService service = new(stores, clock, loggerFactory.CreateLogger<GroupEnrollmentService>());
new Menu(service, stores, clock, dataPath).Run();
```

Конкретні класи створюються лише тут. Решта коду бачить інтерфейси, тому в тестах їх можна замінити фейком і фіксованим годинником.

## Точки розширення

1. Нове сховище (файл, база) — нова реалізація IGroupRepository / IEnrollmentRepository; сервіс не змінюється.
2. Новий вид знижки — нова реалізація IPricingPolicy або нова таблиця в PackageDiscount; сценарії не змінюються.
3. Інше джерело часу (часовий пояс, тестова дата) — нова реалізація IClock.
