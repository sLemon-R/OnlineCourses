# Реєстр дефектів проєктування коду

Модуль: `src/Legacy/LegacyEnrollmentProcessor.cs` (стан до рефакторингу, коміт з тестами).
Номери рядків — за цим файлом. Впорядковано за пріоритетом усунення: спочатку те, що заважає наступним крокам.

| № | Дефект | Місце | Прояв | Рефакторинг |
|---|--------|-------|-------|-------------|
| 1 | Магічне число | Handle 76–91, Preview 129–144, DescribeClient 152 | 7 різних чисел (0.15m, 0.30m, 0.05m, 500m, 1000m, 60m, 10), 25 входжень | Replace Magic Number with Constant |
| 2 | Одержимість елементарними типами | Learner.Kind (11), Handle 32–33, 40, 74, 79; Preview 127, 132; DescribeClient 151 | вид клієнта і стан заявки — рядки "vip", "staff", "new", "paid"; "VIP" компілятор пропустить | Replace Primitive with Enum |
| 3 | Вкладені умови | Handle 36–65 | 4 рівні if, 4 гілки else з return; основний код зсунутий на 20 пробілів | Decompose Conditional, охоронні умови |
| 4 | Довгий метод | Handle 30–115 | 80 рядків, CC = 17, в одному методі перевірка, арифметика, знижки, доставка, звіт і журнал | Extract Method |
| 5 | Дублювання | Handle 73–91 і Preview 126–144 | 19 рядків розрахунку знижки й доставки збігаються з точністю до імен змінних (_tmpDiscount/d, ship/sh) | Extract Method, виклик спільних методів |
| 6 | Великий клас | LegacyEnrollmentProcessor 24–165 | 7 відповідальностей: перевірка, підсумок, знижки, доставка, форматування звіту, журнал, опис клієнта | Extract Class |
| 7 | Довгий список параметрів | Handle 30–34 | 11 параметрів, з них 5 описують одного клієнта (clientId, clientName, clientMail, clientKind, clientDone) | Introduce Parameter Object |
| 8 | Заздрість до чужих даних | DescribeClient 148–157 | 6 звертань до полів Learner (Name, Kind, DoneCount, Email, SinceUtc), 0 — до полів свого класу | Move Method |
| 9 | Тимчасове поле | поля _tmpSum, _tmpDiscount (26–27) | поля класу, які мають сенс лише всередині одного виклику Handle (рядки 67–107) | Remove Temporary Field |
| 10 | Мертвий параметр | Handle 30, 34 | clientId і createdAt передаються, але в тілі методу не читаються жодного разу | Remove Parameter (разом з п. 7) |
