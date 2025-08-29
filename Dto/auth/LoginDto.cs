using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto.auth;

public record class LoginDto([Required][EmailAddress] string Email, [Required] string Password)
{

}
