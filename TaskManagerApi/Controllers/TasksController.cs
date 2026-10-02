using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private static readonly List<TaskItem> Tasks =
    [
        new TaskItem { Id = 1, Title = "Изучить HttpClient", Description = "Настроить IHttpClientFactory и выполнить GET-запросы.", IsCompleted = true },
        new TaskItem { Id = 2, Title = "Реализовать Web API", Description = "Создать API для управления задачами.", IsCompleted = false },
        new TaskItem { Id = 3, Title = "Изучить PATCH", Description = "Сравнить PUT и PATCH для частичного обновления.", IsCompleted = false }
    ];

    [HttpGet]
    public ActionResult<IEnumerable<TaskItem>> GetAll() => Ok(Tasks);

    [HttpGet("{id:int}")]
    public ActionResult<TaskItem> GetById(int id)
    {
        var task = Tasks.FirstOrDefault(item => item.Id == id);
        return task is null ? NotFound(new { message = $"Задача с ID {id} не найдена." }) : Ok(task);
    }

    [HttpPost]
    public ActionResult<TaskItem> Create(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            return BadRequest(new { message = "Название задачи обязательно." });
        }

        task.Id = Tasks.Count == 0 ? 1 : Tasks.Max(item => item.Id) + 1;
        Tasks.Add(task);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, TaskItem updatedTask)
    {
        var task = Tasks.FirstOrDefault(item => item.Id == id);
        if (task is null)
        {
            return NotFound(new { message = $"Задача с ID {id} не найдена." });
        }
        if (string.IsNullOrWhiteSpace(updatedTask.Title))
        {
            return BadRequest(new { message = "Название задачи обязательно." });
        }

        task.Title = updatedTask.Title;
        task.Description = updatedTask.Description;
        task.IsCompleted = updatedTask.IsCompleted;
        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var task = Tasks.FirstOrDefault(item => item.Id == id);
        if (task is null)
        {
            return NotFound(new { message = $"Задача с ID {id} не найдена." });
        }

        Tasks.Remove(task);
        return NoContent();
    }
}
