# Модуль 4. Контейнеризація застосунку (Docker) та розгортання (Deployment на Render)
**Дисципліна:** Практикум з програмної інженерії  
**Проєкт:** `it-department-site` (ASP.NET Core / .NET 10 Web API)  
**Репозиторій на GitHub:** `https://github.com/Diabloid/it-department-site-practicum`  

---

## Розділ 1. Контейнеризація застосунку (Docker)

### 1.1. Багатоступеневий Dockerfile (Multi-stage Build)
Для створення мінімального та безпечного виробничого контейнера використано підхід **Multi-stage Build** у файлі `Dockerfile`:
- **Етап 1 (`build`):** Базовий образ `mcr.microsoft.com/dotnet/sdk:10.0`. На цьому етапі спочатку копіюються лише файли проєктів (`.slnx`, `.csproj`) та виконується `dotnet restore`. Це дозволяє кешувати завантажені NuGet-пакети в окремому шарі Docker. Після цього копіюється решта коду та виконується компіляція релізу: `dotnet publish -c Release -o /app/publish`.
- **Етап 2 (`final`):** Легкий образ середовища виконання `mcr.microsoft.com/dotnet/aspnet:10.0` (без важкого SDK).
- **Безпека:** Застосунок запускається від імені вбудованого непривілейованого користувача `USER app` (захист від підвищення привілеїв усередині контейнера).
- **Порти:** Стандартний порт `8080`, а також підтримка змінної `$PORT` для хмарних провайдерів.

#### Команди для локальної збірки та перевірки образу:
```bash
# 1. Збірка Docker-образу
docker build -t it-department-api:latest .

# 2. Локальний запуск контейнера
docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development it-department-api:latest
```

---

### 1.2. Багатоконтейнерна конфігурація (Docker Compose)
Для локального запуску всієї інфраструктури (Бекенд + СУБД MySQL) створено файл `docker-compose.yml`:
- **Сервіс `api`:** Збирається з локального `Dockerfile`, має прокидання порту `8080:8080`, отримує параметри підключення до БД через внутрішню Docker-мережу (`Server=db;...`) та залежить від стану здоров'я бази (`depends_on: db: condition: service_healthy`).
- **Сервіс `db`:** Використовує офіційний образ `mysql:8.4`. Має налаштований `healthcheck` (`mysqladmin ping`), що гарантує готовність MySQL приймати запити перед стартом бекенда.
- **Том (Volume) `mysql_data`:** Забезпечує персистентність (збереження) даних між перезапусками контейнерів.

#### Команди керування Docker Compose:
```bash
# Запуск комплексу у фоновому режимі
docker compose up --build -d

# Перевірка стану запущених контейнерів
docker compose ps

# Перегляд логів бекенда та бази даних
docker compose logs -f api
docker compose logs -f db

# Зупинка комплексу зі збереженням даних у volume
docker compose down
```

---

### 1.3. Публікація в Container Registry (Docker Hub)
Для передачі зібраного образу у віддалені середовища використовується Docker Hub:
```bash
# 1. Авторизація
docker login

# 2. Тегування образу
docker tag it-department-api:latest YOUR_DOCKERHUB_USERNAME/it-department-api:v1.0.0

# 3. Публікація образу в репозиторій
docker push YOUR_DOCKERHUB_USERNAME/it-department-api:v1.0.0
```

---

## Розділ 2. Розгортання застосунку (Deployment на Render)

Платформа **Render** обрана як основний хмарний хостинг завдяки прямій підтримці контейнеризованих веб-сервісів (Docker Web Service) та декларативній конфігурації через Blueprint (`render.yaml`).

### 2.1. Конфігурація розгортання через `render.yaml` (Render Blueprints)
У корені проєкту розміщено файл специфікації інфраструктури `render.yaml`:
- **Тип:** `web` (Web Service);
- **Середовище виконання:** `runtime: docker`;
- **Шлях до Dockerfile:** `./Dockerfile`;
- **Маршрут перевірки стану (Health Check):** `/api/v1/health` (автоматично очікує відповідь `200 OK` перед перемиканням трафіку);
- **Змінні середовища:** автоматична генерація секретного ключа API `ApiKey__SecretKey` та налаштування `ASPNETCORE_ENVIRONMENT=Production`.

### 2.2. Покрокова інструкція розгортання на Render (1-Click Deployment)
1. **Реєстрація/Вхід:** Відкрити [dashboard.render.com](https://dashboard.render.com/) та увійти через GitHub-акаунт `Diabloid`.
2. **Створення сервісу:**
   - Натиснути кнопку **New +** → вибрати **Blueprint** (або **Web Service**).
   - Підключити публічний репозиторій: `https://github.com/Diabloid/it-department-site-practicum`.
   - Render автоматично зчитає файл `render.yaml`, визначить параметри збірки з `Dockerfile` та підготує веб-сервіс.
3. **Підключення бази даних (За потреби):**
   - У розділі **Environment Variables** веб-сервісу на Render можна вказати змінну `ConnectionStrings__DefaultConnection` до хмарної MySQL (наприклад, безкоштовний інстанс Aiven, PlanetScale або Render MySQL).
   - Якщо рядок підключення не вказано, система автоматично активує вбудований in-memory fallback для демонстраційного перегляду без падіння сервера.
4. **Запуск (Deploy):** Натиснути **Apply / Manual Deploy**. Render запустить збірку `Dockerfile`, виконає перевірку здоров'я через `/api/v1/health` та видасть публічну HTTPS-адресу (наприклад, `https://it-department-api.onrender.com`).

### 2.3. Перевірка працездатності розгорнутого застосунку
Після завершення розгортання перевіряються маршрути:
- `GET https://<your-render-url>/api/v1/health` — повертає `{"status":"Healthy","version":"1.0",...}`.
- `GET https://<your-render-url>/api/v1/disciplines` — повертає публічний кешований каталог дисциплін.
- `GET https://<your-render-url>/api/v2/posts` — повертає розширений список публікацій другої версії API.
- Перевірка Rate Limiting: при надсиланні понад 30 запитів за хвилину сервіс повертає HTTP статус `429 Too Many Requests`.
