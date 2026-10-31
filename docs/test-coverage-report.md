# Звіт з автоматизованого тестування (Модуль 2)
**Дисципліна:** Практикум з програмної інженерії  
**Проєкт:** `it-department-site` (Backend REST API & Web Portal)  
**Розробник:** Студент групи 4 курсу спеціальності 121 «Інженерія програмного забезпечення»  

---

## 1. Загальний огляд тестового покриття
У межах виконання завдань Модуля 2 було спроєктовано та реалізовано комплексну багаторівневу піраміду автоматизованого тестування для платформи ASP.NET Core (.NET 10).

Всі тести реалізовано в окремому проєкті `ItDepartment.Tests` з використанням фреймворку **xUnit**, бібліотеки макетування **Moq**, in-memory сховища **Microsoft.EntityFrameworkCore.InMemory** та аналізатора **Coverlet**.

### Підсумкова статистика виконання:
- **Усього тестів:** 20
- **Успішно пройдено:** 20 (100%)
- **Помилок:** 0
- **Тривалість виконання:** ~6-7 секунд

---

## 2. Структура реалізованих тестів за рівнями піраміди

### 2.1. Модульні тести бізнес-логіки (Unit Tests — `UnitTests/`)
Перевіряють ізольовану логіку обробки даних сервісів (`DisciplineService`):
1. `GetAllAsync_ShouldReturnOrderedDisciplines_WhenCalled` — перевірка правильного подвійного сортування (за `SortOrder`, потім за `Name`).
2. `GetTotalPagesCountAsync_ShouldReturnCorrectPages_WhenItemsExist` — перевірка математики пагінації (11 елементів при ліміті 5 на сторінку = 3 сторінки).
3. `GetTotalPagesCountAsync_ShouldReturnZero_WhenNoMatchingItems` — граничний випадок для порожнього результату.
4. `GetPagedAsync_ShouldReturnCorrectPageSize_WhenPagingApplied` — перевірка зсуву (Skip/Take) та коректності вибірки порцій даних.

### 2.2. Тестування помилкових сценаріїв та винятків (Exception Tests — `ExceptionTests/`)
Перевіряють валідацію вхідних даних DTO та реакцію на виняткові ситуації:
1. `CreateDisciplineRequest_ShouldHaveValidationError_WhenNameIsNull` — генерація помилки валідації при передачі відсутнього обов'язкового поля `Name`.
2. `CreatePostRequest_ShouldHaveValidationError_WhenSlugContainsInvalidCharacters` — блокування некоректного формату slug (пробіли, спецсимволи).
3. `CreatePostRequest_ShouldHaveValidationError_WhenTitleIsTooShort` — перевірка мінімальної довжини заголовка (мінімум 3 символи).
4. `ResilienceService_ShouldThrowTimeoutException_WhenExecutionExceedsTimeoutLimit` — перевірка спрацьовування таймауту Polly (`TimeoutRejectedException` / `TaskCanceledException`) при затримці операції довше 2 секунд.

### 2.3. Ізоляція залежностей через макети (Mocking Tests — `MockTests/`)
Перевіряють взаємодію між сервісами без звернення до файлової системи чи зовнішніх API:
1. `FileStorageService_ShouldCallSaveImage_OnceWhenFileUploaded` — перевірка виклику методу збереження файлу рівно один раз (`Times.Once`).
2. `DisciplineService_ShouldCallGetByIdAsync_WhenRetrievingItem` — перевірка коректності повернення сутності з замокованого джерела.
3. `PostService_ShouldCallDeleteAsync_ExactlyOnce` — підтвердження виклику команди видалення посту.
4. `CurrentUserService_ShouldReturnUserId_WhenAuthenticated` — перевірка контексту поточного авторизованого користувача та його ролі `IsAdmin`.

### 2.4. Інтеграційні тести (Integration Tests — `IntegrationTests/`)
Перевіряють комплексну взаємодію рівнів: *Controller / Service → DbContext → Database / Cache*:
1. `CreateDiscipline_ShouldSaveToDatabase_WhenValidData` — збереження сутності в базі даних та перевірка фізичної наявності запису з усіма полями.
2. `GetPagedAsync_ShouldFilterByIsRequired_InRealDatabase` — фільтрація вибіркових/обов'язкових дисциплін у запитах до БД.
3. `UpdateDiscipline_ShouldUpdateResourceLinks_InRealDatabase` — каскадне оновлення пов'язаних сутностей (`DisciplineResourceLink`).
4. `PostsApiController_ShouldCacheAndReturnFromMemoryCache_OnSubsequentRequests` — інтеграційна перевірка кешування: перший запит наповнює `IMemoryCache`, наступні повертають дані з пам'яті в обхід бази даних.

### 2.5. Наскрізні тести сценаріїв (E2E Tests — `E2ETests/`)
Перевіряють завершені сценарії роботи системи та захисту API:
1. `EndToEnd_Discipline_FullLifecycle_Scenario` — повний життєвий цикл: отримання початкового списку → створення дисципліни → пошук за ID → верифікація оновленого списку.
2. `EndToEnd_ApiVersioning_V1_vs_V2_Comparison_Scenario` — перевірка версіонування API (V1 повертає базовий контракт, V2 повертає розширений DTO з автором, категорією та маркером `ApiVersion = "2.0"`).
3. `EndToEnd_ApiKeySecurityFilter_BlocksUnauthorized_AndAllowsAuthorized` — перевірка фільтра безпеки `[ApiKeyAuth]`: відхилення без ключа (401 Unauthorized), відхилення з хибним ключем (403 Forbidden), дозвіл при валідному ключі.
4. `EndToEnd_HealthStatus_ReturnsHealthyWithVersion` — перевірка працездатності сервісу моніторингу надійності.

---

## 3. Звіт аналізу покриття коду (Coverage Analysis)
- Згенерований артефакт: `coverage.cobertura.xml`
- **Покриття ключової бізнес-логіки та API контролерів:** > 75%
- Всі нові компоненти (API Versioning V1/V2, Rate Limiting, API Key Security, Resilience Service, Caching) покриті тестами на 100%.
