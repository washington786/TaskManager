using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Dto.auth;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<AppUser> userManager, TokenService service) : ControllerBase
    {
        private readonly TokenService _service = service;
        private readonly UserManager<AppUser> _userManager = userManager;

        [HttpPost("signUp")]
        public async Task<ActionResult> Register([FromBody] RegisterDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Field Values" });
            }

            var roleName = string.IsNullOrEmpty(payload.Role) ? "User" : payload.Role;
            if (!new[] { "User", "Admin", "TaskManager" }.Contains(roleName))
            {
                return BadRequest(new { message = "Invalid user role selected" });
            }

            var user = new AppUser()
            {
                Email = payload.Email,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
            };

            if (await _userManager.FindByEmailAsync(payload.Email) is not null)
            {
                return BadRequest(new { message = "Email already exists!" });
            }

            var results = await _userManager.CreateAsync(user, payload.Password);
            var roles = await _userManager.GetRolesAsync(user!);
            var token = _service.GenerateToken(user, roles);

            if (!results.Succeeded)
            {
                return BadRequest(new { message = "Failed to create account", error = results.Errors.Select(e => e.Description) });
            }

            var response = new Dictionary<string, string> { { "Token", token }, { "user", user.ToString() } };

            return Ok(response);
        }

        [HttpPost("signIn")]
        public async Task<ActionResult> Login([FromBody] LoginDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid field values", error = ModelState });
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            var isMatch = await _userManager.CheckPasswordAsync(user!, payload.Password);

            if (user is null || !isMatch)
            {
                return Unauthorized(new { message = "Invalid Email or Password. Please try again." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _service.GenerateToken(user, roles);

            return Ok(new { Token = token, message = "Successfully signed in." });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> PasswordReset([FromBody] ResetDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid field values", error = ModelState });
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user is null)
            {
                return BadRequest(new { message = "Sorry, Email account is not valid!" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return Ok(token);
        }
    }
}
