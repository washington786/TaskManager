using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto.auth;

public record class ResetDto([Required][EmailAddress] string Email, [Required] string NewPassword)
{

}
