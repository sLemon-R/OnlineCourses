# Класова модель OnlineCourses (варіант 17)

## Сутності

| Сутність | Роль | Незмінний | Інваріант |
|---|---|---|---|
| EnrollmentLine | позиція документа (курс у заявці) | так, record | CourseCode не порожній; Seats > 0; UnitPrice >= 0 |
| Enrollment | документ зі станом, агрегат | ні | Id > 0; ListenerId > 0; позиції додаються лише у стані New |
| Listener | довідник учасників (слухач) | так, record | Id > 0; FullName не порожнє; Email містить @ |
| Course | довідник об'єктів обліку (курс) | ні (SeatsLeft змінюється) | Code не порожній; Price >= 0; DurationHours > 0; SeatsLeft >= 0 |
| ArchivedEnrollment | архівна копія заявки, лише читання | так | містить завершену або скасовану заявку |

## Стани заявки

| Із стану | У стан | Умова |
|---|---|---|
| New | Paid | у заявці є хоча б одна позиція |
| New | Cancelled | завжди |
| Paid | InProgress | старт навчання |
| Paid | Cancelled | повернення коштів |
| InProgress | Completed | курс пройдено |
| Completed, Cancelled | — | кінцеві стани, переходів немає |
