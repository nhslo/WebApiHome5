using System.Net;
using System.Net.Http.Json;
using TaskManagerClient.Models;

namespace TaskManagerClient.Services;

public class TaskApiClient(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("TaskApi");

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/tasks", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<TaskItem>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/tasks/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<TaskItem>(cancellationToken: cancellationToken);
    }

    public async Task<TaskItem> CreateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/tasks", task, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<TaskItem>(cancellationToken: cancellationToken)
            ?? throw new TaskApiException("API вернул пустой ответ при создании задачи.");
    }

    public async Task<TaskItem> UpdateAsync(int id, TaskItem task, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/tasks/{id}", task, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<TaskItem>(cancellationToken: cancellationToken)
            ?? throw new TaskApiException("API вернул пустой ответ при обновлении задачи.");
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"api/tasks/{id}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = response.StatusCode switch
        {
            HttpStatusCode.BadRequest => "API отклонил данные. Проверьте заполнение формы.",
            HttpStatusCode.NotFound => "Запрошенная задача не найдена.",
            HttpStatusCode.InternalServerError => "Сервис задач временно недоступен (ошибка 500).",
            _ => $"Ошибка API: {(int)response.StatusCode} {response.ReasonPhrase}."
        };

        throw new TaskApiException(string.IsNullOrWhiteSpace(details) ? message : $"{message} {details}");
    }
}

public sealed class TaskApiException(string message) : Exception(message);
