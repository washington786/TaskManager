namespace TaskManager.Dto.task;

public record class TaskDto(int Id, string Title, string Description, bool IsCompleted, string UserId)
{

}
