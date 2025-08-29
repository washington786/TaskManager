using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto.task;

public record class CreateTaskDto([Required][MaxLength(150)] string Title, string Description, bool IsCompleted)
{

}
