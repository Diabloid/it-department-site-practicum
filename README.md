
# IT Department Site



## 1. Що потрібно встановити

.NET SDK **8.x** 

 MySQL **або** MariaDB | MySQL 8.0+ / MariaDB LTS 

 Git  **будь-яка** 


  

## 2. 

 

**Усі команди `dotnet` виконуються з папки `it_department_site`**, а не з кореня репозиторію.

Інакше буде помилка `Couldn't find a project to run`.

  

```bash

cd it_department_site

```



  

## 3. База даних

  

Застосунок очікує:

db name: `website`

username: `site_admin`

password: `strongpassword`

### Варіант A — Linux (MariaDB)

  

Arch:

  

```bash

sudo pacman -Syu 

sudo pacman -S mariadb

sudo systemctl start mariadb

sudo systemctl enable mariadb

  

sudo mariadb -e "

CREATE DATABASE IF NOT EXISTS website CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE USER IF NOT EXISTS 'site_admin'@'localhost' IDENTIFIED BY 'strongpassword';

GRANT ALL PRIVILEGES ON website.* TO 'site_admin'@'localhost';

FLUSH PRIVILEGES;"

```



  

### Варіант B — Windows (MySQL)

  

1. Встановити **MySQL Server 8.0** через MySQL Installer.

- Config Type: `Development Computer`, порт `3306`

- Authentication: `Use Strong Password Encryption`

- Задати пароль `root`

- Windows Service: залишити автозапуск

2. Відкрити **MySQL Command Line Client** (або Workbench), увійти як `root` і виконати:

  

```sql

CREATE DATABASE website CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE USER 'site_admin'@'localhost' IDENTIFIED BY 'strongpassword';

GRANT ALL PRIVILEGES ON website.* TO 'site_admin'@'localhost';

FLUSH PRIVILEGES;

```

  

> ⚠️ У MySQL 8 `GRANT` **не створює** користувача сам. Спочатку `CREATE USER`, потім `GRANT`,
---

  

## 4. Перевірка
Перевірте наявність файлу `.env` в папці `it_department_site` якщо його немає 
  

## 5. Перший запуск

  

`--migrate` і `--demo-data` — це **аргументи застосунку**, а не команди `dotnet`.

Їх передають після `--`:

  

```bash

cd it_department_site


dotnet run -- --migrate # створює таблиці

dotnet run -- --demo-data # заливає демо-дані
dotnet run # запускає сайт

```

  

`--migrate` і `--demo-data` потрібні **лише один раз** (або після нових міграцій).

Далі достатньо `dotnet run`.

  

Після старту сайт доступний за адресами:

  

- https://localhost:7278

- http://localhost:5186

  

  

Зупинити сайт — `Ctrl+C`.

  

---

  

## 6. Доступ до адмінки

  

Демо-дані створюють користувача `admin` з поштою `admin@example.com` і роллю `Admin`.

Щоб стати адміном, **замініть email адміна на свій корпоративний**.

  

### З терміналу (Linux)

  

```bash

mariadb -u site_admin -pstrongpassword website -e \

"UPDATE users SET email = 'Moniuk.Oleksandr@polytech.cv.ua' WHERE id = 1;"

```

  

### З консолі MariaDB / MySQL

  

Якщо ви вже всередині консолі (`MariaDB [(none)]>` / `mysql>`) — пишіть **тільки SQL**:

  

```sql

USE website;

UPDATE users SET email = 'Moniuk.Oleksandr@polytech.cv.ua' WHERE id = 1;

SELECT * FROM users;

```
