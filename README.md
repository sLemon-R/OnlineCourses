# OnlineCourses

![CI](https://github.com/sLemon-R/OnlineCourses/actions/workflows/ci.yml/badge.svg)

Навчальний проєкт з дисципліни «Конструювання програмного забезпечення».
Варіант № 17: реєстрація слухачів на онлайн-курси.

Застосунок веде заявки слухачів на навчання: у заявку додаються курси (кількість місць і ціна за місце), рахується вартість, заявка проходить стани «нова → оплачена → навчання триває → завершена» або скасовується.

## Склад рішення

- OnlineCourses.Core — логіка предметної області (бібліотека)
- OnlineCourses.App — консольний застосунок (введення, виведення)

## Вимоги

.NET SDK 8.0 або новіший, Git.

## Складання та запуск

```
dotnet build
dotnet run --project OnlineCourses.App
```

## Перевірка якості (ЛР8)

Конвеєр `.github/workflows/ci.yml` запускається на кожен push і pull request. Те саме локально:

```
dotnet format --verify-no-changes && dotnet build -c Release && dotnet test OnlineCourses.Tests -c Release /p:CollectCoverage=true /p:Threshold=60
```

| Крок | Команда | Що ламає збірку |
|---|---|---|
| Формат | `dotnet format --verify-no-changes` | зайвий пробіл, відступ, порушене правило іменування |
| Складання | `dotnet build -c Release` | будь-яке попередження CS, CA, SA, IDE |
| Тести | `dotnet test -c Release` | хоча б один червоний тест |
| Покриття | `/p:Threshold=60` | покриття рядків нижче за 60 % |

Секретів у коді немає; локальні налаштування (`appsettings.Local.json`, `*.secrets.json`, `.env`) ігноруються Git.

## Обробка помилок (ЛР5)

- Помилки вводу користувача повертаються як `Result<T>` з поясненням, застосунок просить повторити ввід.
- Порушення правил предметної області — `DomainRuleException` з ідентифікатором правила.
- Помилки програміста — `ArgumentException` / `InvalidOperationException`, застосунок падає швидко.
- Журнал — `ILogger` (рівні Debug, Information, Warning, Error), аудит у файлі — `FileAuditLog` з `using`.
- Класифікація ситуацій — у файлі docs/errors.md.

## Стандарт кодування

Правила описано у файлі CODING_STANDARD.md і закріплено в .editorconfig.
Перевірка: `dotnet format --verify-no-changes`

## Автор

Сторожук Владислав, група 472
