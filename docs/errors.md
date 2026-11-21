# Класифікація помилок OnlineCourses

| № | Ситуація | Клас | Де виявляється | Стратегія | Повідомлення користувачеві |
|---|---|---|---|---|---|
| 1 | Введено «дві» замість кількості місць | користувача | EnrollmentLineParser.Parse | Result, повторний запит | «Кількість місць — ціле число більше 0» |
| 2 | Ціна введена з комою або від'ємна | користувача | EnrollmentLineParser.Parse | Result, повторний запит | «Ціна — число з крапкою, не менше 0» |
| 3 | Порожній код курсу | користувача | EnrollmentLineParser.Parse | Result, повторний запит | «Код курсу порожній» |
| 4 | Заявки з таким номером немає | користувача | EnrollmentService.Pay | DomainRuleException enrollment.exists | «Заявку 42 не знайдено» |
| 5 | Оплата суми, меншої за вартість | користувача | EnrollmentService.Pay | DomainRuleException enrollment.amount | «Сума менша за вартість заявки» |
| 6 | Оплата вже оплаченої заявки | програміста | Enrollment.ChangeStatus | InvalidOperationException, падати швидко | «Внутрішня помилка STATE-01» |
| 7 | У конструктор сервісу передано null | програміста | EnrollmentService(...) | ArgumentNullException, падати швидко | «Внутрішня помилка INIT-01» |
| 8 | Позиція з Seats = 0 дійшла до ядра | програміста | EnrollmentLine(...) | ArgumentOutOfRangeException | «Внутрішня помилка LINE-01» |
| 9 | Файл журналу аудиту зайнятий іншим процесом | середовища | FileAuditLog(...) | IOException угору, обробка в Main | «Журнал недоступний, роботу продовжено без нього» |
| 10 | Немає прав на запис у теку журналу | середовища | FileAuditLog(...) | UnauthorizedAccessException угору, обробка в Main | «Немає доступу до журналу» |
| 11 | Консоль закрита, Console.ReadLine повертає null | середовища | Program, цикл вводу | завершення вводу без помилки | — |
