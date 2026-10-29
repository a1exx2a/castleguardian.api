# 🏰 Castle Guardian API

![.NET 9](https://img.shields.io/badge/.NET-9.0-blueviolet?style=for-the-badge&logo=dotnet)
![PostgreSQL 17](https://img.shields.io/badge/PostgreSQL-17-blue?style=for-the-badge&logo=postgresql)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-9.0-green?style=for-the-badge)

Web API, моделирующее взаимодействие посетителей с Хранителем замка. Система отслеживает индивидуальный уровень доверия каждого гостя и на его основе принимает решение о пропуске.

## 📖 О проекте

Каждый новый посетитель начинает с нулевого уровня доверия. Взаимодействуя с Хранителем (представляясь, предлагая помощь или угрожая), посетитель изменяет свой рейтинг. Хранитель пропускает в замок только тех, чей уровень доверия достиг **50 и выше**. 

Вся история действий и текущее состояние посетителей надежно сохраняются в базе данных PostgreSQL.

## 🛠 Технологии

* **Платформа:** .NET 9 (ASP.NET Core Web API)
* **База данных:** PostgreSQL 17
* **ORM:** Entity Framework Core 9 (Npgsql)
* **Документация:** Swagger / OpenAPI

## 🚀 Запуск проекта

### Предварительные требования
* Установленный [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* Установленный [PostgreSQL 17](https://www.postgresql.org/download/)
* Глобальный инструмент EF Core: `dotnet tool install --global dotnet-ef`

### Шаги установки

1. **Клонируйте репозиторий:**
   ```bash
   git clone https://github.com/a1exx2a/castleguardian.api.git
   cd CastleGuardian.Api
   ```

2. **Настройте базу данных:**
   Создайте базу данных `GuardianDb` в PostgreSQL (через pgAdmin или psql).
   В файле `appsettings.json` укажите вашу строку подключения:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=GuardianDb;Username=postgres;Password=ваш_пароль"
   }
   ```

3. **Примените миграции:**
   ```bash
   dotnet ef database update
   ```

4. **Запустите приложение:**
   ```bash
   dotnet run
   ```
   Перейдите по адресу `http://localhost:<порт>/swagger` для работы с API.

## ⚙️ Механика и Эндпоинты

### Допустимые действия и изменение доверия:

| Действие | Описание | Изменение доверия |
| :--- | :--- | :---: |
| `Introduce` | Представиться и объяснить цель визита | **+20** |
| `OfferHelp` | Предложить помощь в охране замка | **+30** |
| `Threaten` | Угрожать Хранителю | **-20** |

*(Уровень доверия строго ограничен диапазоном от 0 до 100. Повторное выполнение одного и того же действия одним посетителем запрещено).*

### Маршруты API:

| Метод | Маршрут | Назначение |
| :--- | :--- | :--- |
| **POST** | `/api/visitors` | Зарегистрировать нового посетителя |
| **GET** | `/api/visitors` | Получить список всех посетителей |
| **GET** | `/api/visitors/{id}` | Получить информацию о конкретном посетителе |
| **POST** | `/api/visitors/{id}/actions` | Выполнить действие перед Хранителем |
| **GET** | `/api/visitors/{id}/access` | Проверить возможность прохода в замок |

### Пример запроса действия:

```http
POST /api/visitors/1/actions
Content-Type: application/json

{
  "action": "OfferHelp"
}
```

**Успешный ответ (200 OK):**
```json
{
  "visitorId": 1,
  "action": "OfferHelp",
  "trustLevel": 50
}
```

## 📂 Структура проекта

```text
CastleGuardian.Api/
├── Controllers/       # Обработка HTTP-запросов
├── Data/              # DbContext и настройки EF Core
├── DTO/              # Объекты передачи данных (Data Transfer Objects)
├── Models/            # Сущности базы данных (Visitor, VisitorActionHistory)
├── Services/          # Бизнес-логика (GuardianService)
└── Program.cs         # Точка входа и конфигурация DI
```