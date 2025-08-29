using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }


    public string UserId { get; set; } = string.Empty;

    public AppUser? User { get; set; }
}
