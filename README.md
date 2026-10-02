# WebApiHome5 — Task Manager Client

## О проекте

`TaskManagerClient` — клиентское ASP.NET Core MVC-приложение для управления задачами. Оно отправляет HTTP-запросы во внешний для клиента Web API `TaskManagerApi`. Оба проекта созданы из стандартных шаблонов ASP.NET Core и находятся в одном решении для локального запуска.

Задача содержит `Id`, `Title`, `Description` и `IsCompleted`. Данные демонстрационного API хранятся в памяти и сбрасываются при перезапуске API.

## Адрес API

Локальный адрес API: `http://127.0.0.1:5083/`  
Swagger API: `http://127.0.0.1:5083/swagger`  
Адрес задаётся в `TaskManagerClient/appsettings.json` в параметре `TaskApi:BaseAddress`.

## Запуск

Нужен .NET 8 SDK. Из корня репозитория откройте два окна PowerShell.

Окно 1 — API:

```powershell
dotnet run --project .\TaskManagerApi\TaskManagerApi.csproj --urls http://127.0.0.1:5083
```

Окно 2 — клиент:

```powershell
dotnet run --project .\TaskManagerClient\TaskManagerClient.csproj --urls http://127.0.0.1:5084
```

Откройте клиент: `http://127.0.0.1:5084`.

## HTTP-методы

| Действие | Метод API | Endpoint | Успешный ответ |
|---|---|---|---|
| Показать список | GET | `/api/tasks` | `200 OK` |
| Открыть задачу | GET | `/api/tasks/{id}` | `200 OK` |
| Добавить задачу | POST | `/api/tasks` | `201 Created` |
| Изменить задачу и статус | PUT | `/api/tasks/{id}` | `200 OK` |
| Удалить после подтверждения | DELETE | `/api/tasks/{id}` | `204 No Content` |

Клиент учитывает ошибки `400 Bad Request`, `404 Not Found` и `500 Internal Server Error`, а также проблемы соединения: показывает пользователю понятное сообщение и продолжает работать.

## IHttpClientFactory

В `Program.cs` зарегистрирован именованный клиент `TaskApi` через `AddHttpClient`. Его `BaseAddress` берётся из конфигурации. `TaskApiClient` получает `IHttpClientFactory` через DI и создаёт настроенный клиент методом `CreateClient("TaskApi")`. Таким образом, приложение повторно использует управляемую фабрикой конфигурацию и не создаёт новый `HttpClient` вручную перед запросами.

## Скриншоты выполнения

Все снимки сделаны в браузере во время выполнения реальных действий с клиентом и API.

### Список задач — GET `/api/tasks`

![Список задач, полученный из Web API](docs/tasks-list.png)

### Просмотр задачи — GET `/api/tasks/1`

![Подробности задачи](docs/task-details.png)

### Добавление задачи — форма и POST

![Заполненная форма добавления задачи](docs/task-create-form.png)

![Задача создана и появилась в списке](docs/task-created.png)

### Изменение задачи — форма и PUT

![Редактирование названия, описания и статуса](docs/task-edit-form.png)

![Изменённая задача и статус в списке](docs/task-updated.png)

### Удаление задачи — подтверждение и DELETE

![Экран подтверждения перед удалением](docs/delete-confirmation.png)

![Подтверждённое удаление задачи](docs/task-deleted.png)

### Обработка 404

![Понятное сообщение, если задача отсутствует](docs/task-not-found.png)

## Вывод

Создано рабочее MVC-приложение, которое получает и изменяет задачи через отдельный Web API. `IHttpClientFactory` предоставляет именованный HTTP-клиент с заданным базовым адресом, а пользовательские формы позволяют выполнить полный CRUD-цикл.
