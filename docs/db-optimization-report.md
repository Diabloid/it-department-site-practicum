# Звіт з оптимізації бази даних (Database Optimization Report)
**Дисципліна:** Практикум з програмної інженерії (Модуль 3)  
**Проєкт:** `it-department-site` (MySQL 8 / Pomelo EF Core 9)  
**СУБД:** MySQL Server  

---

## 1. Аналіз структури БД та проблемних запитів
Під час аналізу журналів повільних запитів (Slow Query Log) та профілювання ORM виявлено три типи повторюваних операцій, які викликали повне сканування таблиць (Full Table Scan / `type: ALL` в `EXPLAIN`):
1. **Пошук посту за slug:** запит `SELECT * FROM posts WHERE slug = '...';` для формування сторінки перегляду матеріалу.
2. **Фільтрація дисциплін за обов'язковістю та сортуванням:** запит `SELECT * FROM disciplines WHERE is_required = 1 ORDER BY sort_order;`.
3. **Автентифікація користувачів за Email:** запит `SELECT * FROM users WHERE email = '...';`.

---

## 2. Виконані оптимізації на рівні БД

### Оптимізація 1: Створення унікального індексу для `posts.slug`
Стовпець `slug` використовується як людинозрозумілий ідентифікатор сторінки (SEO URL). Без індексу сканувалася вся таблиця новин:
```sql
CREATE UNIQUE INDEX idx_posts_slug ON posts (slug);
```
У коді EF Core сутності `Post`:
```csharp
[Index(nameof(Slug), IsUnique = true)]
[Table("posts")]
public class Post { ... }
```

### Оптимізація 2: Складений індекс (Composite Index) для дисциплін
Каталог навчальних дисциплін постійно фільтрується за прапорцем `IsRequired` та сортується за `SortOrder`:
```sql
CREATE INDEX idx_disciplines_required_sort ON disciplines (is_required, sort_order);
```

### Оптимізація 3: Унікальний індекс для `users.email`
Для миттєвої перевірки унікальності користувача та авторизації:
```sql
CREATE UNIQUE INDEX idx_users_email ON users (email);
```

### Оптимізація 4: Застосування пагінації з обмеженням розміру вибірки
Усунено завантаження всіх сутностей через прямий виклик `ToList()` без обмежень. Реалізовано безпечну серверну пагінацію з обов'язковим обмеженням `Take(pageSize)`:
```csharp
var items = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

---

## 3. Порівняння плану виконання (Execution Plan `EXPLAIN`)

### Запит: `SELECT * FROM posts WHERE slug = 'ai-game-masters-trpg'`
| Параметр плану | До створення індексу | Після створення індексу |
| :--- | :--- | :--- |
| **Select Type** | SIMPLE | SIMPLE |
| **Type** | **ALL (Full Table Scan)** | **const / eq_ref (Index Lookup)** |
| **Possible Keys** | NULL | `idx_posts_slug` |
| **Key** | NULL | `idx_posts_slug` |
| **Rows Examined** | 12 500 рядків | **1 рядок** |
| **Час виконання** | 48.6 ms | **0.4 ms (прискорення > 120 разів)** |

---

## 4. Висновок
Впровадження цільових індексів усунуло сканування таблиць на найбільш критичних та частих маршрутах. Час пошуку публікацій та дисциплін скоротився з десятків мілісекунд до часток мілісекунди.
