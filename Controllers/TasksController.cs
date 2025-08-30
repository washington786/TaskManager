using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Dto.task;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController(ApplicationDbContext dbContext, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly ApplicationDbContext _db = dbContext;
        private readonly UserManager<AppUser> _userManager = userManager;

        [HttpPost()]
        public async Task<ActionResult> CreateTask([FromBody] CreateTaskDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var task = new TaskItem()
            {
                Description = payload.Description,
                Title = payload.Title,
                UserId = user.Id,
                User = user,
                IsCompleted = payload.IsCompleted
            };

            await _db.AddAsync(task);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTaskById),
                new { id = task.Id },
                new TaskDto(
                    task.Id,
                   task.Title,
                    task.Description,
                   task.IsCompleted,
                   task.UserId)
            );
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAlltasks()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user!);

            IQueryable<TaskItem> query = (IQueryable<TaskItem>)_db.Tasks;

            if (!roles.Contains("Admin") && !roles.Contains("TaskManager"))
            {
                query = query.Where(t => t.UserId == user!.Id);
            }

            var tasks = await query.Select(e =>
                new TaskDto(
                    e.Id,
                    e.Title,
                    e.Description!,
                    e.IsCompleted,
                    e.UserId
                )).ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            var task = await _db.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            if (!roles.Contains("Admin") && !roles.Contains("TaskManager") && task.UserId != user.Id)
            {
                return Forbid();
            }

            var response = new TaskItem()
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                UserId = task.UserId,
            };
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto payload)
        {
            if (id != payload.Id) return BadRequest();

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            var task = await _db.Tasks.FindAsync(id);

            if (!roles.Contains("Admin") && !roles.Contains("TaskManager") && task.UserId != user.Id) return Forbid();

            task.Description = payload.Description;
            task.Title = payload.Title;
            task.IsCompleted = payload.IsComplete;

            _db.Update(task);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            var task = await _db.Tasks.FindAsync(id);

            if (task == null) return NotFound();

            if (!roles.Contains("Admin") && !roles.Contains("TaskManager") && task.UserId != user.Id) return Forbid();

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
