using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto.task;

public record class UpdateTaskDto([Required] int Id, [Required] string Title, string Description, bool IsComplete)
{

}
