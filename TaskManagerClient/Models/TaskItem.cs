using System.ComponentModel.DataAnnotations;

namespace TaskManagerClient.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название задачи.")]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Выполнена")]
    public bool IsCompleted { get; set; }
}
