using Microsoft.AspNetCore.Mvc;
using TaskManagerClient.Models;
using TaskManagerClient.Services;

namespace TaskManagerClient.Controllers;

public class TasksController(TaskApiClient api, ILogger<TasksController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            ViewBag.ApiAddress = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["TaskApi:BaseAddress"];
            return View(await api.GetAllAsync(cancellationToken));
        }
        catch (TaskApiException ex)
        {
            logger.LogWarning(ex, "Could not load tasks from API");
            ViewBag.Error = ex.Message;
            return View(Array.Empty<TaskItem>());
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Task API is unavailable");
            ViewBag.Error = "Не удалось связаться с API задач. Убедитесь, что TaskManagerApi запущен.";
            return View(Array.Empty<TaskItem>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        try
        {
            var task = await api.GetByIdAsync(id, cancellationToken);
            if (task is null)
            {
                TempData["Error"] = $"Задача с ID {id} не найдена (404).";
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }
        catch (Exception ex) when (ex is TaskApiException or HttpRequestException)
        {
            logger.LogError(ex, "Could not get task {TaskId}", id);
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult Create() => View(new TaskItem());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskItem task, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(task);
        try
        {
            await api.CreateAsync(task, cancellationToken);
            TempData["Success"] = "Задача создана.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is TaskApiException or HttpRequestException)
        {
            logger.LogError(ex, "Could not create task");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(task);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        try
        {
            var task = await api.GetByIdAsync(id, cancellationToken);
            if (task is null)
            {
                TempData["Error"] = $"Задача с ID {id} не найдена (404).";
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }
        catch (Exception ex) when (ex is TaskApiException or HttpRequestException)
        {
            logger.LogError(ex, "Could not load task {TaskId} for editing", id);
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskItem task, CancellationToken cancellationToken)
    {
        if (id != task.Id) return BadRequest();
        if (!ModelState.IsValid) return View(task);
        try
        {
            await api.UpdateAsync(id, task, cancellationToken);
            TempData["Success"] = "Изменения сохранены.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is TaskApiException or HttpRequestException)
        {
            logger.LogError(ex, "Could not update task {TaskId}", id);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(task);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var task = await api.GetByIdAsync(id, cancellationToken);
            if (task is null)
            {
                TempData["Error"] = $"Задача с ID {id} не найдена (404).";
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }
        catch (Exception ex) when (ex is TaskApiException or HttpRequestException)
        {
            logger.LogError(ex, "Could not load task {TaskId} for deletion", id);
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        try
        {
            await api.DeleteAsync(id, cancellationToken);
            TempData["Success"] = "Задача удалена.";
        }
        catch (Exception ex) when (ex is TaskApiException or HttpRequestException)
        {
            logger.LogError(ex, "Could not delete task {TaskId}", id);
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
