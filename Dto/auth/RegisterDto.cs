using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto.auth;

public record class RegisterDto([Required][EmailAddress] string Email, [Required][MinLength(6)] string Password, [Required][MinLength(3)][MaxLength(100)] string FirstName, [Required][MinLength(3)][MaxLength(100)] string LastName, string Role = "User")
{

}
