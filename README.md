# OnlineCourses

![CI](https://github.com/sLemon-R/OnlineCourses/actions/workflows/ci.yml/badge.svg)

Консольний застосунок для зарахування слухачів на онлайн-курси: групи з обмеженими місцями, передумови курсів, знижка за пакет курсів і звіт про наповнюваність груп.

## Автор і дисципліна

Варіант № 17 (OnlineCourses), Сторожук Владислав, група 472. Дисципліна «Конструювання програмного забезпечення», самостійна робота № 3.

## Швидкий старт

```
git clone https://github.com/sLemon-R/OnlineCourses.git
dotnet build -c Release OnlineCourses/OnlineCourses.sln
dotnet run --project OnlineCourses/OnlineCourses.App
```

## Структура рішення

- `OnlineCourses.Core` — домен (Domain), прикладний шар (Services, Abstractions, Reporting, Input) та інфраструктура (Infrastructure) окремими теками.
- `OnlineCourses.App` — консольне меню, стартові дані, файловий журнал, композиційний корінь.
- `OnlineCourses.Tests` — модульні тести xUnit бібліотеки логіки.
- `src/Legacy`, `tests/Legacy.Tests` — успадкований модуль з ЛР6 і його характеризаційні тести.

## Можливості

Сценарії описано в [docs/spec.md](docs/spec.md):

- UC-01 Зарахувати слухача до однієї чи кількох груп (контроль місць, передумов, знижка за пакет).
- UC-02 Відрахувати слухача або перевести в іншу групу того самого курсу.
- UC-03 Оплатити заявку.
- UC-04 Знайти заявки за слухачем і станом.
- UC-05 Звіт «наповнюваність груп».
- UC-06 Зберегти групи у `data/groups.json`.

Посібник користувача — [docs/user-guide.md](docs/user-guide.md), архітектура — [docs/architecture.md](docs/architecture.md).

## Тести й покриття

```
dotnet test OnlineCourses.Tests -c Release /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=70
```

Звіт `coverage.cobertura.xml` з'являється в теці `OnlineCourses.Tests`. Поточне покриття рядків бібліотеки логіки — дивіться останній прогін конвеєра; поріг 70 %.

Усе, що перевіряє конвеєр, одним рядком локально:

```
dotnet format --verify-no-changes && dotnet build -c Release && dotnet test OnlineCourses.Tests -c Release /p:CollectCoverage=true /p:Threshold=70
```

## Технології

.NET 8, C# 12, xUnit, coverlet, аналізатори Roslyn (.NET Analyzers, StyleCop.Analyzers), System.Text.Json, Microsoft.Extensions.Logging, GitHub Actions.

## Обмеження й відомий борг

Заявки зберігаються лише в пам'яті, у файл JSON пишуться тільки групи. Немає розмежування прав: усі дії виконує один адміністратор. Повернення коштів після старту навчання не реалізоване. Правило залежностей між шарами контролюється оглядом коду, а не окремими проєктами. Повний перелік — у [docs/self-check.md](docs/self-check.md).
