# MiddlewareDemo (ASP.NET Core, .NET 8)

Відкрити `MiddlewareDemo.sln` у Visual Studio 2022 і натиснути F5.

| # | Middleware | Файл |
|---|-----------|------|
| 1 | Час отримання/відправки + тривалість (мс) | RequestTimingMiddleware.cs |
| 2 | METHOD URL STATUS TIME у консоль | ConsoleLoggingMiddleware.cs |
| 3 | Чорний список IP -> 403 | IpBlockMiddleware.cs |
| 4 | Лічильник `Request #N: GET /path` | RequestCounterMiddleware.cs |
| 5 | X-API-KEY -> 401 | ApiKeyMiddleware.cs |
| 6 | Serilog -> `logs/log-YYYYMMDD.txt` | Program.cs |

Налаштування (ключ API, список заблокованих IP) — у `appsettings.json`.
Тестові запити — у `requests.http`.
